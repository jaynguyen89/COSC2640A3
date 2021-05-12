import { combineReducers } from 'redux';

import authenticationStore from '../features/authentication/redux/reducer';
import accountStore from '../features/homepage/redux/reducer';

export default combineReducers({
    authenticationStore,
    accountStore
});