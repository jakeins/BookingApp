import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ResourceService } from '../../services/resource.service';
import { Resource } from '../../models/resource';
import { Logger } from '../../services/logger.service';
import { Router, ActivatedRoute } from '@angular/router';
import { FolderService } from '../../services/folder.service';
import { Folder } from '../../models/folder';
import { DevService } from '../../services/development.service';

@Component({
  selector: 'app-resource-edit',
  templateUrl: './resource-edit.component.html',
  styleUrls: ['./resource-edit.component.css']
})

export class ResourceEditComponent implements OnInit {

    

  constructor(
    private fb: FormBuilder,
    private resourceService: ResourceService,
    private actRoute: ActivatedRoute,
    private folderService: FolderService
  )
  { }

  folders: Folder[] = [this.folderService.newRoot()];
  resourceForm: FormGroup;
  parentFolderId: number;
  updateMode: boolean;
  get createMode(): boolean { return !this.updateMode; };



  ngOnInit() {

    this.updateMode = +this.actRoute.snapshot.params['id'] > 0;

    let title: string;
    let description: string;
    let ruleId: number;
    let folderId: number;
    let isActive: boolean;



    if (this.updateMode) {


      Logger.warn('Update mode!');

    }
    else if (this.createMode) {

      //#region Dev only
      title = DevService.RandomTerm('wishAdj') + ' ' + DevService.RandomTerm('basicColor') + ' ' + DevService.RandomTerm('popNoun') + ' ' + DevService.RandomTerm('emojiNature');
      description = DevService.GenerateText(3, 30);
      ruleId = 1;
      //folderId
      isActive = true;
      //#endregion Dev only

      //reading parent folder from query parameters
      let pFoldId = this.actRoute.snapshot.queryParams["parentFolderId"];
      this.parentFolderId = +pFoldId > 0 ? +pFoldId : 0;

    }

    //form initializing
    this.resourceForm = this.fb.group({
      title: title,
      description: description,
      ruleId: ruleId,
      folderId: folderId,
      isActive: isActive ? 'true' : 'false',
    });

    //getting list of folders
    this.folderService.getList().subscribe((result: Folder) => {
      for (let key in result) {
        this.folders.push(result[key]);
      }

      //forcing folder seleciton during creation
      if (this.createMode)
        this.resourceForm.controls['folderId'].setValue(this.parentFolderId);

    });
  }






  onSubmit() {
    let form = this.resourceForm.value;
    let model = new Resource();

    model.title = form.title;
    model.description = form.description;
    model.isActive = form.isActive;
    model.ruleId = form.ruleId;
    model.folderId = form.folderId;

    if (model.folderId == 0)
      model.folderId = null;

    this.resourceService.createResource(model)
      .subscribe(result => {
        let resourceId = result['resourceId'];

        Logger.log(model);

        Logger.log(`Resource ${resourceId} has been created.`);

        //this.router.navigate(['/resources/'+resourceId]);

      }, error => console.log(error));
  }

}

