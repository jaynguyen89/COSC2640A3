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