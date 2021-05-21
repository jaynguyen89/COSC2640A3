import {IAuthUser} from "../../authentication/redux/interfaces";
import {EMPTY_STRING, IActionResult, IStatusMessage} from "../../../providers/helpers";

export const EMPTY_NUMBER = -1;


export interface IManageClassroomContent {
    authUser: IAuthUser,
    invokeGetClassContent: (auth: IAuthUser, classroomId: string | null) => void,
    getClassContent: IActionResult,
    clearAuthUser: () => void,
}

//IClassContent = To Display Files
export interface IClassContent {
    id: string,
    videos: Array<IFile>,
    audios: Array<IFile>
}

export interface IFile {
    id: string,
    name: string,
    type: number,
    extension: string,
    uploadedOn: string
}
//IRichContent = To Display RichContent
export interface IRichContent {
    classroomID:string,
    htmlContent:string
}

export const defaultRichContent : IRichContent = {
    classroomID:EMPTY_STRING,
    htmlContent:EMPTY_STRING
}

export const defaultFileContent: IFile = {
    id: EMPTY_STRING,
    name: EMPTY_STRING,
    type: EMPTY_NUMBER,
    extension: EMPTY_STRING,
    uploadedOn: EMPTY_STRING
}

export const EMPTY_IFILE = [defaultFileContent];

export const defaultClassroomContent : IClassContent = {
    id: EMPTY_STRING,
    videos: EMPTY_IFILE,
    audios: EMPTY_IFILE
}

export interface IRichContentImport {
    classroomID:string,
    filesForImport: Array<any>
}

export interface IContentBinding {
    classroomID:string,
    fileType: number
}

export interface IFileAdding extends IContentBinding{
    uploadedFiles: Array<any>
}

export interface IUpdateFile extends IFileAdding {
    removedFiles: Array<string>
}

export interface IClassroomContentModal {
    selectedClassroomContent: IClassContent,
    statusMessage: IStatusMessage,
    closeAlert: () => void,
    task: string,
    isTaskRunning: boolean,
    handleAddBtn: (newFile: IFile) => void,
}
