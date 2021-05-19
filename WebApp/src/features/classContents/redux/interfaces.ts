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