import * as classroomConstants from "./constants";
import produce from 'immer';
import {DEFAULT_ACTION_RESULT, IActionResult} from "../../../providers/helpers";

interface IClassroomStore {
    getTeacherClassrooms: IActionResult,
    removeClassroom: IActionResult,
    completedClassroom: IActionResult,
    getClassroomDetail: IActionResult,
    createClassroom: IActionResult,
    updateClassroom: IActionResult,
    uploadFileImport: IActionResult
}

const initialState : IClassroomStore = {
    getTeacherClassrooms: DEFAULT_ACTION_RESULT,
    removeClassroom: DEFAULT_ACTION_RESULT,
    completedClassroom: DEFAULT_ACTION_RESULT,
    getClassroomDetail: DEFAULT_ACTION_RESULT,
    createClassroom: DEFAULT_ACTION_RESULT,
    updateClassroom: DEFAULT_ACTION_RESULT,
    uploadFileImport: DEFAULT_ACTION_RESULT
}

const reducer = produce((state, action) => {
    switch (action.type) {
        case classroomConstants.GET_TEACHER_CLASSROOMS_REQUEST_SENT:
            state.getTeacherClassrooms.action = classroomConstants.GET_TEACHER_CLASSROOMS_REQUEST_SENT;
            state.getTeacherClassrooms.payload = null;
            state.getTeacherClassrooms.error = null;
            return;
        case classroomConstants.GET_TEACHER_CLASSROOMS_REQUEST_SUCCESS:
            state.getTeacherClassrooms.action = classroomConstants.GET_TEACHER_CLASSROOMS_REQUEST_SUCCESS;
            state.getTeacherClassrooms.payload = action.payload;
            state.getTeacherClassrooms.error = null;
            return;
        case classroomConstants.GET_TEACHER_CLASSROOMS_REQUEST_FAILED:
            state.getTeacherClassrooms.action = classroomConstants.GET_TEACHER_CLASSROOMS_REQUEST_FAILED;
            state.getTeacherClassrooms.payload = null;
            state.getTeacherClassrooms.error = action.error;
            return;
        case classroomConstants.REMOVE_CLASSROOM_REQUEST_SENT:
            state.removeClassroom.action = classroomConstants.REMOVE_CLASSROOM_REQUEST_SENT;
            state.removeClassroom.payload = null;
            state.removeClassroom.error = null;
            return;
        case classroomConstants.REMOVE_CLASSROOM_REQUEST_SUCCESS:
            state.removeClassroom.action = classroomConstants.REMOVE_CLASSROOM_REQUEST_SUCCESS;
            state.removeClassroom.payload = action.payload;
            state.removeClassroom.error = null;
            return;
        case classroomConstants.REMOVE_CLASSROOM_REQUEST_FAILED:
            state.removeClassroom.action = classroomConstants.REMOVE_CLASSROOM_REQUEST_FAILED;
            state.removeClassroom.payload = null;
            state.removeClassroom.error = action.error;
            return;
        case classroomConstants.COMPLETED_CLASSROOM_REQUEST_SENT:
            state.completedClassroom.action = classroomConstants.COMPLETED_CLASSROOM_REQUEST_SENT;
            state.completedClassroom.payload = null;
            state.completedClassroom.error = null;
            return;
        case classroomConstants.COMPLETED_CLASSROOM_REQUEST_SUCCESS:
            state.completedClassroom.action = classroomConstants.COMPLETED_CLASSROOM_REQUEST_SUCCESS;
            state.completedClassroom.payload = action.payload;
            state.completedClassroom.error = null;
            return;
        case classroomConstants.COMPLETED_CLASSROOM_REQUEST_FAILED:
            state.completedClassroom.action = classroomConstants.COMPLETED_CLASSROOM_REQUEST_FAILED;
            state.completedClassroom.payload = null;
            state.completedClassroom.error = action.error;
            return;
        case classroomConstants.GET_CLASSROOM_DETAILS_REQUEST_SENT:
            state.getClassroomDetail.action = classroomConstants.GET_CLASSROOM_DETAILS_REQUEST_SENT;
            state.getClassroomDetail.payload = null;
            state.getClassroomDetail.error = null;
            return;
        case classroomConstants.GET_CLASSROOM_DETAILS_REQUEST_SUCCESS:
            state.getClassroomDetail.action = classroomConstants.GET_CLASSROOM_DETAILS_REQUEST_SUCCESS;
            state.getClassroomDetail.payload = action.payload;
            state.getClassroomDetail.error = null;
            return;
        case classroomConstants.GET_CLASSROOM_DETAILS_REQUEST_FAILED:
            state.getClassroomDetail.action = classroomConstants.GET_CLASSROOM_DETAILS_REQUEST_FAILED;
            state.getClassroomDetail.payload = null;
            state.getClassroomDetail.error = action.error;
            return;
        case classroomConstants.CREATE_CLASSROOM_REQUEST_SENT:
            state.createClassroom.action = classroomConstants.CREATE_CLASSROOM_REQUEST_SENT;
            state.createClassroom.payload = null;
            state.createClassroom.error = null;
            return;
        case classroomConstants.CREATE_CLASSROOM_REQUEST_SUCCESS:
            state.createClassroom.action = classroomConstants.CREATE_CLASSROOM_REQUEST_SUCCESS;
            state.createClassroom.payload = action.payload;
            state.createClassroom.error = null;
            return;
        case classroomConstants.CREATE_CLASSROOM_REQUEST_FAILED:
            state.createClassroom.action = classroomConstants.CREATE_CLASSROOM_REQUEST_FAILED;
            state.createClassroom.payload = null;
            state.createClassroom.error = action.error;
            return;
        case classroomConstants.UPDATE_CLASSROOM_REQUEST_SENT:
            state.updateClassroom.action = classroomConstants.UPDATE_CLASSROOM_REQUEST_SENT;
            state.updateClassroom.payload = null;
            state.updateClassroom.error = null;
            return;
        case classroomConstants.UPDATE_CLASSROOM_REQUEST_SUCCESS:
            state.updateClassroom.action = classroomConstants.UPDATE_CLASSROOM_REQUEST_SUCCESS;
            state.updateClassroom.payload = action.payload;
            state.updateClassroom.error = null;
            return;
        case classroomConstants.UPDATE_CLASSROOM_REQUEST_FAILED:
            state.updateClassroom.action = classroomConstants.UPDATE_CLASSROOM_REQUEST_FAILED;
            state.updateClassroom.payload = null;
            state.updateClassroom.error = action.error;
            return;
        case classroomConstants.UPLOAD_JSON_CLASSROOMS_FILE_REQUEST_SENT:
            state.uploadFileImport.action = classroomConstants.UPLOAD_JSON_CLASSROOMS_FILE_REQUEST_SENT;
            state.uploadFileImport.payload = null;
            state.uploadFileImport.error = null;
            return;
        case classroomConstants.UPLOAD_JSON_CLASSROOMS_FILE_REQUEST_SUCCESS:
            state.uploadFileImport.action = classroomConstants.UPLOAD_JSON_CLASSROOMS_FILE_REQUEST_SUCCESS;
            state.uploadFileImport.payload = action.payload;
            state.uploadFileImport.error = null;
            return;
        case classroomConstants.UPLOAD_JSON_CLASSROOMS_FILE_REQUEST_FAILED:
            state.uploadFileImport.action = classroomConstants.UPLOAD_JSON_CLASSROOMS_FILE_REQUEST_FAILED;
            state.uploadFileImport.payload = null;
            state.uploadFileImport.error = action.error;
            return;
        default:
            return;
    }
}, initialState);

export default reducer;