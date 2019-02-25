import { Component, OnInit } from '@angular/core';

import { TreeEntry } from '../../models/tree-entry';
import { TreeNode } from '../../models/tree-node';
import { ResourceService } from '../../services/resource.service';
import { FolderService } from '../../services/folder.service';
import { Resource } from '../../models/resource';
import { Folder } from '../../models/folder';
import { Logger } from '../../services/logger.service';
import { AuthService } from '../../services/auth.service';


@Component({
  selector: 'app-tree',
  templateUrl: './tree.component.html',
  styleUrls: ['./tree.component.css']
})
export class TreeComponent implements OnInit {

  resourceEntries: TreeEntry[] = [];
  folderEntries: TreeEntry[] = [];
  allEntries: TreeEntry[] = [];
  treeFlat: TreeNode[] = [];
  treeRoot = new TreeNode(0, "[root]", new TreeEntry(new Folder(0,"root", null)), []);
  private barrierCount = 0;

  authService: AuthService;
  isUser: boolean;
  isAdmin: boolean;

  constructor(
    private resourceService: ResourceService,
    private folderService: FolderService,
    authService: AuthService
  ) {
    this.authService = authService;
  }

  ngOnInit() {
    this.isUser = true;
    this.folderService.getList().subscribe((result: Folder) => {
      for (let key in result) {
        let entry = new TreeEntry(result[key]);
        this.folderEntries[key] = entry;
        this.allEntries.push(entry);
      }
      this.reachDoubleBarrier();
    });

    this.resourceService.getResources().subscribe((result: Resource[]) => {
      for (let key in result) {
        let entry = new TreeEntry(result[key]);
        this.resourceEntries[key] = entry;
        this.allEntries.push(entry);
      }
      this.resourceService.resetOccupancies(this.resourceEntries);
      this.reachDoubleBarrier();
    });
  }

  
  reachDoubleBarrier() {
    this.barrierCount++;

    if (this.barrierCount == 2) {
      this.buildTree();
    }
  }

  buildTree() {
    //sort everyting by id
    this.allEntries.sort(function (a, b) { return a.id - b.id });

    //nodify everything and stick into the root
    let folderNodeMap: TreeNode[] = [];
    let allNodeMap: TreeNode[] = [];
        
    this.treeRoot.children = [];
    for (let entry of this.allEntries) {

      let node = new TreeNode(0, (entry.isFolder ? `[${entry.title}]` : entry.title), entry, []);

      if (entry.isFolder === true)
        folderNodeMap[entry.id] = node;

      allNodeMap.push(node);

      this.treeRoot.children.push(node);
    }

    //stick the nodes on
    for (let currentNode of allNodeMap) {
      let targetParent = currentNode.item.parentFolderId;

      if (targetParent != null && folderNodeMap[targetParent] != undefined) {
        folderNodeMap[targetParent].children.push(currentNode);
        this.treeRoot.children.splice(this.treeRoot.children.indexOf(currentNode),1);       
      }
    }

    //resursively fill nesting and form flat list of nodes
    this.nestAndFlatten(this.treeRoot);
  }

  nestAndFlatten(pNode: TreeNode) {
    this.treeFlat.push(pNode);
    for (let cNode of pNode.children) {
      cNode.nesting = pNode.nesting + 1;
      this.nestAndFlatten(cNode);
    }
  }
}
