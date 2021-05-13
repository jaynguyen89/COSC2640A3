import {IAuthUser} from "../../authentication/redux/interfaces";
import {EMPTY_STRING, IActionResult} from "../../../providers/helpers";
import {invokeCompletedClassroomsRequest, invokeRemoveClassroomsRequest} from "./actions";

export interface IManageClassroom {
    authUser: IAuthUser,
    invokeGetAllTeacherClassroomsRequest: (auth: IAuthUser, teacherId: string | null) => void,
    getTeacherClassrooms: IActionResult,
    clearAuthUser: () => void,
    invokeRemoveClassroomsRequest: (auth: IAuthUser, classroomId: string) => void,
    invokeCompletedClassroomsRequest: (auth: IAuthUser, classroomId: string) => void,
    removeClassroom: IActionResult,
    completedClassroom: IActionResult
}

export interface IClassroom {
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

export const defaultClassroomDetail : IClassroomDetail = {
    capacity: 0,
    commencedOn: EMPTY_STRING,
    duration: 0,
    durationUnit: 0,
    isActive: false,
    createdOn: EMPTY_STRING,
    normalizedDuration: EMPTY_STRING
}

export const defaultClassroom : IClassroom = {
    id: EMPTY_STRING,
    teacherId: EMPTY_STRING,
    teacherName: EMPTY_STRING,
    className: EMPTY_STRING,
    price: 0,
    enrolmentsCount: 0,
    classroomDetail: defaultClassroomDetail
}

export interface IClassroomCard {
    classroom: IClassroom,
    completed?: true,
    handleTitleClicked: (classroomId: string) => void,
    handleReviseBtn?: (classroomId: string) => void,
    handleRemoveBtn?: (classroomId: string) => void,
    handleMarkAsCompletedBtn?: (classroomId: string) => void
}

export interface IClassroomModal {
    selectedClassroomId: string,
    task: string,
    handleCreateBtn: (newClassroom: IClassroom) => void,
    handleUpdateBtn: (updatedClassroom: IClassroom) => void
}