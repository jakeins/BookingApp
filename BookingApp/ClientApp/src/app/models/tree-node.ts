import { TreeEntry } from "./tree-entry";

export class TreeNode {
  constructor(
    public item?: TreeEntry,
    public children?: TreeNode[]
  ) {}
}
