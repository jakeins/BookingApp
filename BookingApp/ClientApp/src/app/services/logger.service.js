"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var Logger = /** @class */ (function () {
    function Logger() {
    }
    Logger.log = function (msg) { console.log(msg); };
    Logger.error = function (msg) { console.error(msg); };
    Logger.warn = function (msg) { console.warn(msg); };
    return Logger;
}());
exports.Logger = Logger;
//# sourceMappingURL=logger.service.js.map