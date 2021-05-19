import * as teacherConstants from './constants';
import produce from 'immer';
import {DEFAULT_ACTION_RESULT, IActionResult} from "../../../providers/helpers";

interface ITeacherStore {
    exportClassrooms: IActionResult,
    exportStudents: IActionResult,
    getSchedules: IActionResult,
    updateMarks: IActionResult
}

const initialState: ITeacherStore = {
    exportClassrooms: DEFAULT_ACTION_RESULT,
    exportStudents: DEFAULT_ACTION_RESULT,
    getSchedules: DEFAULT_ACTION_RESULT,
    updateMarks: DEFAULT_ACTION_RESULT
}

const reducer = produce((state, action) => {
    switch (action.type) {
        case teacherConstants.EXPORT_CLASSROOMS_REQUEST_SENT:
            state.exportClassrooms.action = teacherConstants.EXPORT_CLASSROOMS_REQUEST_SENT;
            state.exportClassrooms.payload = null;
            state.exportClassrooms.error = null;
            return;
        case teacherConstants.EXPORT_CLASSROOMS_REQUEST_SUCCESS:
            state.exportClassrooms.action = teacherConstants.EXPORT_CLASSROOMS_REQUEST_SUCCESS;
            state.exportClassrooms.payload = action.payload;
            state.exportClassrooms.error = null;
            return;
        case teacherConstants.EXPORT_CLASSROOMS_REQUEST_FAILED:
            state.exportClassrooms.action = teacherConstants.EXPORT_CLASSROOMS_REQUEST_FAILED;
            state.exportClassrooms.payload = null;
            state.exportClassrooms.error = action.error;
            return;
        case teacherConstants.EXPORT_STUDENTS_REQUEST_SENT:
            state.exportStudents.action = teacherConstants.EXPORT_STUDENTS_REQUEST_SENT;
            state.exportStudents.payload = null;
            state.exportStudents.error = null;
            return;
        case teacherConstants.EXPORT_STUDENTS_REQUEST_SUCCESS:
            state.exportStudents.action = teacherConstants.EXPORT_STUDENTS_REQUEST_SUCCESS;
            state.exportStudents.payload = action.payload;
            state.exportStudents.error = null;
            return;
        case teacherConstants.EXPORT_STUDENTS_REQUEST_FAILED:
            state.exportStudents.action = teacherConstants.EXPORT_STUDENTS_REQUEST_FAILED;
            state.exportStudents.payload = null;
            state.exportStudents.error = action.error;
            return;
        case teacherConstants.GET_IMPORT_SCHEDULES_PROGRESS_REQUEST_SENT:
            state.getSchedules.action = teacherConstants.GET_IMPORT_SCHEDULES_PROGRESS_REQUEST_SENT;
            state.getSchedules.payload = null;
            state.getSchedules.error = null;
            return;
        case teacherConstants.GET_IMPORT_SCHEDULES_PROGRESS_REQUEST_SUCCESS:
            state.getSchedules.action = teacherConstants.GET_IMPORT_SCHEDULES_PROGRESS_REQUEST_SUCCESS;
            state.getSchedules.payload = action.payload;
            state.getSchedules.error = null;
            return;
        case teacherConstants.GET_IMPORT_SCHEDULES_PROGRESS_REQUEST_FAILED:
            state.getSchedules.action = teacherConstants.GET_IMPORT_SCHEDULES_PROGRESS_REQUEST_FAILED;
            state.getSchedules.payload = null;
            state.getSchedules.error = action.error;
            return;
        case teacherConstants.ADD_MARKS_TO_ENROLMENT_REQUEST_SENT:
            state.updateMarks.action = teacherConstants.ADD_MARKS_TO_ENROLMENT_REQUEST_SENT;
            state.updateMarks.payload = null;
            state.updateMarks.error = null;
            return;
        case teacherConstants.ADD_MARKS_TO_ENROLMENT_REQUEST_SUCCESS:
            state.updateMarks.action = teacherConstants.ADD_MARKS_TO_ENROLMENT_REQUEST_SUCCESS;
            state.updateMarks.payload = action.payload;
            state.updateMarks.error = null;
            return;
        case teacherConstants.ADD_MARKS_TO_ENROLMENT_REQUEST_FAILED:
            state.updateMarks.action = teacherConstants.ADD_MARKS_TO_ENROLMENT_REQUEST_FAILED;
            state.updateMarks.payload = null;
            state.updateMarks.error = action.error;
            return;
        default:
            return;
    }
}, initialState);

export default reducer;