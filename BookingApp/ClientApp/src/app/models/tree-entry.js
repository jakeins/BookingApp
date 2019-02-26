"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var TreeEntry = /** @class */ (function () {
    function TreeEntry(original) {
        this.isFolder = original.parentFolderId !== undefined;
        this.id = original.id;
        this.title = original.title;
        this.isActive = original.isActive;
        if (this.isFolder) {
            this.parentFolderId = original.parentFolderId;
        }
        else {
            this.parentFolderId = original.folderId;
        }
        //Logger.log(`${original.title} - ${this.isFolder}`);
    }
    return TreeEntry;
}());
exports.TreeEntry = TreeEntry;
//# sourceMappingURL=tree-entry.js.map