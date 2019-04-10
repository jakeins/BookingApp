import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { RuleService } from '../../../services/rule.service';
import { MatTableDataSource, MatSort, MatPaginator } from '@angular/material';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material';
import { RuleComponent } from '../rule/rule.component';
import { NotificationService } from '../../../services/notification.service';
import { ActivatedRoute} from '@angular/router';

@Component({
  selector: 'app-rule-list',
  templateUrl: './rule-list.component.html',
  styleUrls: ['./rule-list.component.css']
})
export class RuleListComponent implements OnInit {
 @ViewChild(MatSort) sort: MatSort;
 @ViewChild(MatPaginator) paginator: MatPaginator;
  listData: MatTableDataSource<any>;
  searchKey:string;
  displayColumns: string[] = ['id', 'title', 'minTime', 'maxTime', 'serviceTime', 'isActive', 'actions'];
  error: string;
  id: number;
  idEdit: number;


  constructor(
    private service: RuleService,
    private dialog: MatDialog,
    private notificationService: NotificationService,
    private activatedRoute: ActivatedRoute
    ) { }

  ngOnInit() {
    
    if(this.activatedRoute.snapshot.url.pop().path == 'edit')
      this.idEdit = this.activatedRoute.snapshot.params['id'];
    else
      this.id = this.activatedRoute.snapshot.params['id'];
    this.updateTable();
 
    if(this.id != null || this.idEdit != null){
        setTimeout(()=> {
          if(this.id != null)
            this.onDetails(this.id, this.id);
          else
            this.onEdit(this.idEdit);
        }, 0);
    }
  }

  onSearchReset(){
    this.searchKey = '';
  }

  applyFilter(){
    this.listData.filter = this.searchKey.trim().toLocaleLowerCase();
  }

  onCreate(){
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.width = "60%";
    const dialogRef = this.dialog.open(RuleComponent, dialogConfig);
    dialogRef.afterClosed().subscribe(res =>{
      this.updateTable();
    })
  }
  
  onDetails(rowId: number, readonlymode: number){
    let dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.width = "60%";
    dialogConfig.data = { ruleId: rowId, readonlymode: readonlymode};
    const dialogRef  = this.dialog.open(RuleComponent, dialogConfig); 
    dialogRef.afterClosed().subscribe(res =>{
      this.updateTable();
    })
  }
  onEdit(rowId: number){
    let dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.width = "60%";
    dialogConfig.data = { ruleId: rowId, readonlymode: null};
    const dialogRef  = this.dialog.open(RuleComponent, dialogConfig); 
    dialogRef.afterClosed().subscribe(res =>{
      this.updateTable();
    })
  }
 
  onDelete(rowId: number){
    if(confirm('Are u sure to delete rule')){
    this.service.deleteRule(rowId).subscribe(res =>{
      this.updateTable();
    },err => { 
      this.error = err.status + ': ' + err.error.Message + '.';
    });
    this.notificationService.delete('Deleted successfully!');
    }
  }

  onClear(){
    this.error = null;
  }

  updateTable(){
    this.service.getRules().subscribe( res => {
      this.listData = new MatTableDataSource();
      this.listData.data = res;
      this.listData.sort = this.sort;
      this.listData.paginator = this.paginator;
      this.listData.filterPredicate = (data, filter) =>{                                      //filter only by table columns cells
        const dataStr = data.title.toLowerCase() + data.id + data.minTime + data.maxTime + data.serviceTime + data.isActive;
        return dataStr.indexOf(filter) != -1;
      }
    })
  }
}