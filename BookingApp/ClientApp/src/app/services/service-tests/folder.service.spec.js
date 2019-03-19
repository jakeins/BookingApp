"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/common/http/testing");
var testing_2 = require("@angular/core/testing");
var folder_service_1 = require("../folder.service");
var globals_1 = require("../../globals");
var folder_1 = require("../../models/folder");
describe('FolderService', function () {
    var service;
    var httpMock;
    var BaseUrlFolder = globals_1.BASE_API_URL + '/folder';
    beforeEach(function () {
        testing_2.TestBed.configureTestingModule({
            imports: [testing_1.HttpClientTestingModule],
            providers: [folder_service_1.FolderService],
        });
        // inject the service
        service = testing_2.TestBed.get(folder_service_1.FolderService);
        httpMock = testing_2.TestBed.get(testing_1.HttpTestingController);
    });
    it('should get the correct folders', function () {
        service.getList().subscribe(function (data) {
            expect(data.length).toBe(2);
        });
        var req = httpMock.expectOne(BaseUrlFolder, 'call to api');
        expect(req.request.method).toBe('GET');
        req.flush([new folder_1.Folder('Town Hall'), new folder_1.Folder('Town City')]);
        httpMock.verify();
    });
    it('should get the correct folder by id = 1', function () {
        service.getFolder(1).subscribe(function (data) {
            expect(data.title).toBe('Town Hall');
        });
        var req = httpMock.expectOne(BaseUrlFolder + '/1', 'call to api');
        expect(req.request.method).toBe('GET');
        req.flush(new folder_1.Folder('Town Hall'));
        httpMock.verify();
    });
    it('should post the correct data', function () {
        service.createFolder(new folder_1.Folder('Town Hall')).subscribe();
        var req = httpMock.expectOne(BaseUrlFolder, 'post to api');
        expect(req.request.method).toBe('POST');
        httpMock.verify();
    });
    it('should put the correct data', function () {
        service.updateFolder(new folder_1.Folder('Town Hall', null, null, null, 1)).subscribe();
        var req = httpMock.expectOne(BaseUrlFolder + '/1', 'put to api');
        expect(req.request.method).toBe('PUT');
        httpMock.verify();
    });
    it('should delete the correct data', function () {
        service.deleteFolder(1).subscribe();
        var req = httpMock.expectOne(BaseUrlFolder + '/1', 'delete to api');
        expect(req.request.method).toBe('DELETE');
        httpMock.verify();
    });
});
//# sourceMappingURL=folder.service.spec.js.map