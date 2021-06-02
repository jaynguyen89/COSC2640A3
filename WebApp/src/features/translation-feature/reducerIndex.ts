import { combineReducers } from 'redux';

import authenticationStore from '../features/authentication/redux/reducer';
import accountStore from '../features/homepage/redux/reducer';
import classroomStore from '../features/classroom/redux/reducer';
import teacherStore from '../features/teacher/redux/reducer';
import studentStore from '../features/student/redux/reducer';
import checkoutStore from '../features/checkout/redux/reducer';
import contentStore from '../features/classContents/redux/reducer';
import translationStore from '../features/translation/redux/reducer';

export default combineReducers({
    authenticationStore,
    accountStore,
    classroomStore,
    teacherStore,
    studentStore,
    checkoutStore,
    contentStore,
    translationStore
});