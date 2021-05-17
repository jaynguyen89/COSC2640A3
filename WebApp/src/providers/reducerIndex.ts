import { combineReducers } from 'redux';

import authenticationStore from '../features/authentication/redux/reducer';
import accountStore from '../features/homepage/redux/reducer';
import classroomStore from '../features/classroom/redux/reducer';
import teacherStore from '../features/teacher/redux/reducer';

export default combineReducers({
    authenticationStore,
    accountStore,
    classroomStore,
    teacherStore
});