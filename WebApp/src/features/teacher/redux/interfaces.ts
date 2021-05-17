import {IAuthUser} from "../../authentication/redux/interfaces";
import {IFileImport} from "../../classroom/redux/interfaces";
import {IActionResult, IStatusMessage} from "../../../providers/helpers";

export interface IImportExport {
    authUser: IAuthUser,
    invokeUploadFileForImportRequest: (auth: IAuthUser, fileData: IFileImport) => void,
    uploadFileImport: IActionResult,
    invokeGetSchedulesProgressRequest: (auth: IAuthUser) => void,
    getSchedules: IActionResult,
    clearAuthUser: () => void
}

export interface IDataExport {
    classroomIds: Array<string>
}

export interface IImportData {
    selectedFile: IFileImport,
    handleSelection: (file: FileList | null, type: string) => void,
    attemptUploadingFileForImport: () => void,
    enableButtons: boolean,
    shouldShowSpinner: boolean,
    statusMessage: IStatusMessage,
    closeStatusMessage: () => void
}

export interface IExportData {
    authUser: IAuthUser,
    invokeGetAllTeacherClassroomsRequest: (auth: IAuthUser, teacherId: string | null) => void,
    getTeacherClassrooms: IActionResult,
    clearAuthUser: () => void,
    invokeExportClassroomsRequest: (auth: IAuthUser, dataExport: IDataExport) => void,
    invokeExportStudentsRequest: (auth: IAuthUser, dataExport: IDataExport) => void,
    exportClassrooms: IActionResult,
    exportStudents: IActionResult
}

export interface IScheduleList {
    schedules: Array<ISchedule>,
    statusMessage: IStatusMessage,
    closeStatusMessage: () => void,
    shouldShowSpinner: boolean,
}

export interface ISchedule {
    id: string,
    accountId: string,
    fileId: string,
    fileName: string,
    fileSize: number,
    uploadedOn: number,
    status: number,
    isForClassroom: boolean
}

export const getSchedule = (data: any) => {
    return {
        id: data.id,
        accountId: data.accountId,
        fileId: data.fileId,
        fileName: data.fileName,
        fileSize: data.fileSize,
        uploadedOn: data.uploadedOn,
        status: data.status,
        isForClassroom: false
    } as ISchedule;
}