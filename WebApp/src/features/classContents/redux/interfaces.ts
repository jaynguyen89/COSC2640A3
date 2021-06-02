import {IAuthUser} from "../../authentication/redux/interfaces";
import {EMPTY_STRING, IActionResult} from "../../../providers/helpers";

export interface IManageClassContent {
    authUser: IAuthUser,
    clearAuthUser: () => void,
    getContent: IActionResult,
    addFiles: IActionResult,
    updateFiles: IActionResult,
    addRichContent: IActionResult,
    importRichContent: IActionResult,
    updateRichContent: IActionResult,
    invokeGetClassroomContentRequest: (auth: IAuthUser, classroomId: string) => void,
    invokeAddFilesRequest: (auth: IAuthUser, filesData: IFileAdding) => void,
    invokeUpdateFilesRequest: (auth: IAuthUser, filesData: IFileUpdating) => void,
    invokeAddRichContentRequest: (auth: IAuthUser, richContent: IRichContent) => void,
    invokeImportRichContentRequest: (auth: IAuthUser, richContentImport: IRichContentImport) => void,
    invokeUpdateRichContentRequest: (auth: IAuthUser, richContent: IRichContent) => void
}

export interface IClassContent {
    id: string,
    videos: Array<IFile>,
    audios: Array<IFile>,
    photos: Array<IFile>,
    attachments: Array<IFile>,
    htmlContent: string
}

export const defaultClassroomContent: IClassContent = {
    id: EMPTY_STRING,
    videos: Array<IFile>(),
    audios: Array<IFile>(),
    photos: Array<IFile>(),
    attachments: Array<IFile>(),
    htmlContent: EMPTY_STRING
}

export interface IFile {
    id: string,
    name: string,
    type: number,
    extension: string,
    uploadedOn: string
}

export const emptyFile: IFile = {
    id: EMPTY_STRING,
    name: EMPTY_STRING,
    type: 0,
    extension: EMPTY_STRING,
    uploadedOn: EMPTY_STRING
}

interface ClassroomBinding {
    classroomId: string,
    fileType: number
}

export interface IFileAdding extends ClassroomBinding {
    uploadedFiles: FileList
}

export const defaultAddedFiles: IFileAdding = {
    classroomId: EMPTY_STRING,
    fileType: 0,
    uploadedFiles: null as unknown as FileList
}

export interface IFileUpdating extends IFileAdding {
    removedFiles: Array<string>
}

export const defaultUpdatedFiles: IFileUpdating = {
    classroomId: EMPTY_STRING,
    fileType: 0,
    removedFiles: Array<string>(),
    uploadedFiles: null as unknown as FileList
}

export interface IRichContent {
    classroomId: string,
    htmlContent: string
}

export const defaultContent: IRichContent = {
    classroomId: EMPTY_STRING,
    htmlContent: EMPTY_STRING
}

export interface IRichContentImport {
    classroomId: string,
    filesForImport: FileList
}

export const defaultContentImport: IRichContentImport = {
    classroomId: EMPTY_STRING,
    filesForImport: null as unknown as FileList
}

export interface IUpdateFilesModal {
    classroomId: string,
    files: Array<IFile>,
    fileType: number,
    selectedFiles: Array<string>,
    handleFilesInput: (files: FileList) => void,
    removeBtnClick: (fileId: string) => void,
    unselectBtnClick: (fileId: string) => void,
    updateBtnClick: () => void
}

export interface IAddFilesInput {
    files: FileList,
    fileType: number,
    handleFilesInput: (files: FileList, fileType: number) => void,
    confirmAdding: () => void
}

export interface IContentEditor {
    content: string,
    informChanges: (newContent: any) => void
}