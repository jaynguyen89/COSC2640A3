import { combineReducers } from 'redux';

import authenticationStore from '../authentication/redux/reducer';
import accountStore from '../account/redux/reducer';
import messageStore from '../forum/redux/reducer';
import bigQueryStore from '../bigquery/redux/reducer';

export default combineReducers({
    authenticationStore,
    accountStore,
    messageStore,
    bigQueryStore
});