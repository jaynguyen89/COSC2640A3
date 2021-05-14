import {IAuthUser} from "../../authentication/redux/interfaces";
import {EMPTY_STRING, IActionResult, IStatusMessage} from "../../../providers/helpers";
import moment from "moment";

export interface IManageClassroom {
    authUser: IAuthUser,
    invokeGetAllTeacherClassroomsRequest: (auth: IAuthUser, teacherId: string | null) => void,
    getTeacherClassrooms: IActionResult,
    clearAuthUser: () => void,
    invokeRemoveClassroomsRequest: (auth: IAuthUser, classroomId: string) => void,
    invokeCompletedClassroomsRequest: (auth: IAuthUser, classroomId: string) => void,
    removeClassroom: IActionResult,
    completedClassroom: IActionResult,
    invokeGetClassroomDetailRequest: (auth: IAuthUser, classroomId: string) => void,
    getClassroomDetail: IActionResult,
    invokeCreateClassroomsRequest: (auth: IAuthUser, classroom: IClassroom) => void,
    invokeUpdateClassroomsRequest: (auth: IAuthUser, classroom: IClassroom) => void,
    createClassroom: IActionResult,
    updateClassroom: IActionResult
}

export interface IClassroomData {
    id: string,
    teacherId: string,
    teacherName: string,
    className: string,
    price: number,
    enrolmentsCount: number,
    classroomDetail: IClassroomDetail
}

export interface IClassroomDetail {
    capacity: number,
    commencedOn: string,
    duration: number,
    durationUnit: number,
    isActive: boolean,
    createdOn: string,
    normalizedDuration: string
}

export interface IClassroom {
    id?: string,
    className: string,
    price: number,
    capacity: number,
    commencedOn: string,
    duration: number,
    durationUnit: number
}

export const defaultClassroomDetail : IClassroomDetail = {
    capacity: 0,
    commencedOn: moment().format(),
    duration: 0,
    durationUnit: 0,
    isActive: false,
    createdOn: EMPTY_STRING,
    normalizedDuration: EMPTY_STRING
}

export const defaultClassroom : IClassroomData = {
    id: EMPTY_STRING,
    teacherId: EMPTY_STRING,
    teacherName: EMPTY_STRING,
    className: EMPTY_STRING,
    price: 0,
    enrolmentsCount: 0,
    classroomDetail: defaultClassroomDetail
}

export interface IClassroomCard {
    classroom: IClassroomData,
    selectedClassroomId: string,
    completed?: true,
    handleTitleClicked: (classroomId: string) => void,
    handleReviseBtn?: (classroomId: string) => void,
    handleRemoveBtn?: (classroomId: string) => void,
    handleMarkAsCompletedBtn?: (classroomId: string) => void
}

export interface IClassroomModal {
    selectedClassroom: IClassroomData,
    statusMessage: IStatusMessage,
    closeAlert: () => void,
    task: string,
    isTaskRunning: boolean,
    handleCreateBtn: (newClassroom: IClassroomData) => void,
    handleUpdateBtn: (updatedClassroom: IClassroomData) => void
}

export const getClassroom = (classroom: IClassroomData): IClassroom => {
    return {
        className: classroom.className,
        price: classroom.price,
        capacity: classroom.classroomDetail.capacity,
        commencedOn: classroom.classroomDetail.commencedOn,
        duration: classroom.classroomDetail.duration,
        durationUnit: classroom.classroomDetail.durationUnit
    } as IClassroom;
}