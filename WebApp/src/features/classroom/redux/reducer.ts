import * as classroomConstants from "./constants";
import produce from 'immer';
import {DEFAULT_ACTION_RESULT, IActionResult} from "../../../providers/helpers";

interface IClassroomStore {
    getTeacherClassrooms: IActionResult,
    removeClassroom: IActionResult,
    completedClassroom: IActionResult
}

const initialState : IClassroomStore = {
    getTeacherClassrooms: DEFAULT_ACTION_RESULT,
    removeClassroom: DEFAULT_ACTION_RESULT,
    completedClassroom: DEFAULT_ACTION_RESULT
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
        case classroomConstants.REMOVE_CLASSROOMS_REQUEST_SENT:
            state.removeClassroom.action = classroomConstants.REMOVE_CLASSROOMS_REQUEST_SENT;
            state.removeClassroom.payload = null;
            state.removeClassroom.error = null;
            return;
        case classroomConstants.REMOVE_CLASSROOMS_REQUEST_SUCCESS:
            state.removeClassroom.action = classroomConstants.REMOVE_CLASSROOMS_REQUEST_SUCCESS;
            state.removeClassroom.payload = action.payload;
            state.removeClassroom.error = null;
            return;
        case classroomConstants.REMOVE_CLASSROOMS_REQUEST_FAILED:
            state.removeClassroom.action = classroomConstants.REMOVE_CLASSROOMS_REQUEST_FAILED;
            state.removeClassroom.payload = null;
            state.removeClassroom.error = action.error;
            return;
        case classroomConstants.COMPLETED_CLASSROOMS_REQUEST_SENT:
            state.completedClassroom.action = classroomConstants.COMPLETED_CLASSROOMS_REQUEST_SENT;
            state.completedClassroom.payload = null;
            state.completedClassroom.error = null;
            return;
        case classroomConstants.COMPLETED_CLASSROOMS_REQUEST_SUCCESS:
            state.completedClassroom.action = classroomConstants.COMPLETED_CLASSROOMS_REQUEST_SUCCESS;
            state.completedClassroom.payload = action.payload;
            state.completedClassroom.error = null;
            return;
        case classroomConstants.COMPLETED_CLASSROOMS_REQUEST_FAILED:
            state.completedClassroom.action = classroomConstants.COMPLETED_CLASSROOMS_REQUEST_FAILED;
            state.completedClassroom.payload = null;
            state.completedClassroom.error = action.error;
            return;
        default:
            return;
    }
}, initialState);

export default reducer;