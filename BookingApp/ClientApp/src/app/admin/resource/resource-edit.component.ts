import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ResourceService } from '../../services/resource.service';
import { Resource } from '../../models/resource';
import { Logger } from '../../services/logger.service';
import { Router, ActivatedRoute } from '@angular/router';
import { FolderService } from '../../services/folder.service';
import { Folder } from '../../models/folder';
import { DevService } from '../../services/development.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-resource-edit',
  templateUrl: './resource-edit.component.html',
  styleUrls: ['./resource-edit.component.css']
})

export class ResourceEditComponent implements OnInit {

  constructor(
    private fb: FormBuilder,
    private resourceService: ResourceService,
    private router: Router,
    private actRoute: ActivatedRoute,
    private folderService: FolderService,
    private authService: AuthService
  )
  { }

  folders: Folder[] = [this.folderService.newRoot()];
  resourceForm: FormGroup;
  parentFolderId: number;
  updateMode: boolean;
  get createMode(): boolean { return !this.updateMode; };
  model: Resource;
  apiError: string;
  id: number;
  authChangedSubscription: any;

  ngOnDestroy() {
    this.authChangedSubscription.unsubscribe();
  };

  ngOnInit() {

    this.authChangedSubscription = this.authService.AuthChanged.subscribe(() => {
      if (!this.authService.isAdmin) {
        this.router.navigate(['/error/401']);
      }
    });

    this.id = +this.actRoute.snapshot.params['id'];

    this.updateMode = this.id > 0;

    if (this.updateMode) {

      this.resourceService.getResource(this.id).subscribe((response: Resource) => {
        if (response.folderId == undefined)
          response.folderId = 0;
        this.model = response;
        this.initializeForm();
      }, error => { this.router.navigate(['/error/401']); });

    }
    else if (this.createMode) {

      this.model = new Resource();

      //#region DEVELOPMENT
      this.model.title = [DevService.RandomTerm('wishAdj'), DevService.RandomTerm('basicColor'), DevService.RandomTerm('popNoun'), DevService.RandomTerm('emojiNature')].join(' ');
      this.model.description = DevService.GenerateText(3, 30);
      this.model.ruleId = 1;
      this.model.folderId = 0;
      this.model.isActive = true;
      //#endregion DEVELOPMENT

      //reading parent folder from query parameters
      let pFoldId = this.actRoute.snapshot.queryParams["parentFolderId"];
      this.parentFolderId = +pFoldId > 0 ? +pFoldId : 0;

      this.initializeForm();
    }

    //getting list of folders
    this.folderService.getList().subscribe((result: Folder) => {
      for (let key in result) {
        this.folders.push(result[key]);
      }

      //folder preseleciton during creation
      if (this.createMode)
        this.resourceForm.controls['folderId'].setValue(this.parentFolderId);

    });
  }



  initializeForm() {
    this.resourceForm = this.fb.group({
      title: [this.model.title, [Validators.required, Validators.minLength(3), Validators.maxLength(64)]],
      description: [this.model.description, Validators.maxLength(512)],
      ruleId: this.model.ruleId,
      folderId: this.model.folderId,
      isActive: this.model.isActive ? 'true' : 'false',
    });

    Logger.log('Form initialized.');
    Logger.log(this.resourceForm);
  }

  onSubmit() {
    this.apiError = undefined;

    let formData = this.resourceForm.value;

    this.model.title = formData.title;
    this.model.description = formData.description;
    this.model.isActive = formData.isActive == 'true';
    this.model.ruleId = formData.ruleId > 1 ? formData.ruleId : 1;
    this.model.folderId = formData.folderId > 0 ? formData.folderId : null;

    Logger.log(this.model);

    if (this.updateMode) {
      this.resourceService.updateResource(this.model)
        .subscribe(result => {
          Logger.log(`Resource has been updated on ${result['updatedTime']}.`);

          this.router.navigate(['/resources', this.model.id]);

        }, error => this.handleError(error));
    }
    else if (this.createMode) {
      this.resourceService.createResource(this.model)
        .subscribe(result => {
          let resourceId = result['resourceId'];

          Logger.log(`Resource ${resourceId} has been created on ${result['updatedTime']}.`);

          this.router.navigate(['/resources', resourceId]);

        }, error => this.handleError(error));
    }
  }

  delete() {
    this.resourceService.deleteResource(this.id).subscribe(() => {
      Logger.warn(`Resource ${this.id} deleted.`);
      this.router.navigate(['']);
    }, error => this.handleError(error));
  }


  handleError(error: any ) {
    console.log(error);
    this.apiError = error['status'];

    if (error['error'] != undefined)
      this.apiError += ': ' + error['error']['Message'];
  }

}
