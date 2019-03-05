"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var Folder = /** @class */ (function () {
    function Folder(title, parentFolderId, defaultRuleId, isActive, id) {
        if (isActive === void 0) { isActive = false; }
        this.title = title;
        this.parentFolderId = parentFolderId;
        this.defaultRuleId = defaultRuleId;
        this.isActive = isActive;
        this.id = id;
    }
    return Folder;
}());
exports.Folder = Folder;
//# sourceMappingURL=folder.js.map