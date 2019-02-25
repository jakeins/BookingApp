import { TreeEntry } from "./tree-entry";

export class TreeNode {
  nesting: number;
  title: string;
  item?: TreeEntry;
  children?: TreeNode[];
  constructor(nesting: number, title: string, item?: TreeEntry, children?: TreeNode[]) {
    this.nesting = nesting;
    this.title = title;
    this.item = item;
    this.children = children;
  }
}
