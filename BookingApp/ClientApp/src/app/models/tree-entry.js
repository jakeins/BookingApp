"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var TreeEntry = /** @class */ (function () {
    function TreeEntry(id, title, isFolder, isActive, parentFolderId) {
        this.id = id;
        this.title = title;
        this.isFolder = isFolder;
        this.isActive = isActive;
        this.parentFolderId = parentFolderId;
    }
    return TreeEntry;
}());
exports.TreeEntry = TreeEntry;
//# sourceMappingURL=tree-entry.js.map