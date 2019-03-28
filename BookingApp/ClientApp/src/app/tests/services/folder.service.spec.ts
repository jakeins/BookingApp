import {
  HttpClientTestingModule,
  HttpTestingController,
} from '@angular/common/http/testing';
import { } from 'jasmine';
import { TestBed } from '@angular/core/testing';
import { FolderService } from '../../services/folder.service';
import { BASE_API_URL } from '../../globals';
import { Folder } from '../../models/folder';


describe('FolderService tests', () => {
  let service: FolderService;
  let httpMock: HttpTestingController;
  let BaseUrlFolder = BASE_API_URL + '/folder';

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [FolderService],
    });

    // inject the service
    service = TestBed.get(FolderService);
    httpMock = TestBed.get(HttpTestingController);
  });


  it('should get the correct folders', () => {
    service.getList().subscribe((data: Folder[]) => {
      expect(data.length).toBe(2);
    });

    const req = httpMock.expectOne(BaseUrlFolder, 'call to api');
    expect(req.request.method).toBe('GET');
    req.flush([new Folder('Town Hall'), new Folder('Town City')]);
    httpMock.verify();
  });


  it('should get the correct folder by id = 1', () => {
    service.getFolder(1).subscribe((data: Folder) => {
      expect(data.title).toBe('Town Hall');
    });

    const req = httpMock.expectOne(BaseUrlFolder + '/1', 'call to api');
    expect(req.request.method).toBe('GET');
    req.flush(new Folder('Town Hall'));
    httpMock.verify();
  });


  it('should post the correct data', () => {
    service.createFolder(new Folder('Town Hall')).subscribe();

    const req = httpMock.expectOne(BaseUrlFolder, 'post to api');
    expect(req.request.method).toBe('POST');

    httpMock.verify();
  });


  it('should put the correct data', () => {
    service.updateFolder(new Folder('Town Hall', null, null, null, 1)).subscribe();

    const req = httpMock.expectOne(BaseUrlFolder + '/1', 'put to api');
    expect(req.request.method).toBe('PUT');
    httpMock.verify();
  });


  it('should delete the correct data', () => {
    service.deleteFolder(1).subscribe();

    const req = httpMock.expectOne(BaseUrlFolder + '/1', 'delete to api');
    expect(req.request.method).toBe('DELETE');
    httpMock.verify();
  });

});
