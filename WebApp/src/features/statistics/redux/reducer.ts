import * as statisticsConstants from './constants';
import produce from 'immer';
import {DEFAULT_ACTION_RESULT, IActionResult} from "../../../providers/helpers";

interface IStatisticsStore {
    getStatistics: IActionResult,
    triggerMapper: IActionResult,
    triggerReducer: IActionResult
}

const initialState: IStatisticsStore = {
    getStatistics: DEFAULT_ACTION_RESULT,
    triggerMapper: DEFAULT_ACTION_RESULT,
    triggerReducer: DEFAULT_ACTION_RESULT
}

const reducer = produce((state, action) => {
    switch (action.type) {
        case statisticsConstants.GET_STATISTICS_RESULTS_REQUEST_SENT:
            state.getStatistics.action = statisticsConstants.GET_STATISTICS_RESULTS_REQUEST_SENT;
            state.getStatistics.payload = null;
            state.getStatistics.error = null;
            return;
        case statisticsConstants.GET_STATISTICS_RESULTS_REQUEST_SUCCESS:
            state.getStatistics.action = statisticsConstants.GET_STATISTICS_RESULTS_REQUEST_SUCCESS;
            state.getStatistics.payload = action.payload;
            state.getStatistics.error = null;
            return;
        case statisticsConstants.GET_STATISTICS_RESULTS_REQUEST_FAILED:
            state.getStatistics.action = statisticsConstants.GET_STATISTICS_RESULTS_REQUEST_FAILED;
            state.getStatistics.payload = null;
            state.getStatistics.error = action.error;
            return;
        case statisticsConstants.TRIGGER_MAPPER_REQUEST_SENT:
            state.triggerMapper.action = statisticsConstants.TRIGGER_MAPPER_REQUEST_SENT;
            state.triggerMapper.payload = null;
            state.triggerMapper.error = null;
            return;
        case statisticsConstants.TRIGGER_MAPPER_REQUEST_SUCCESS:
            state.triggerMapper.action = statisticsConstants.TRIGGER_MAPPER_REQUEST_SUCCESS;
            state.triggerMapper.payload = action.payload;
            state.triggerMapper.error = null;
            return;
        case statisticsConstants.TRIGGER_MAPPER_REQUEST_FAILED:
            state.triggerMapper.action = statisticsConstants.TRIGGER_MAPPER_REQUEST_FAILED;
            state.triggerMapper.payload = null;
            state.triggerMapper.error = action.error;
            return;
        case statisticsConstants.TRIGGER_REDUCER_REQUEST_SENT:
            state.triggerReducer.action = statisticsConstants.TRIGGER_REDUCER_REQUEST_SENT;
            state.triggerReducer.payload = null;
            state.triggerReducer.error = null;
            return;
        case statisticsConstants.TRIGGER_REDUCER_REQUEST_SUCCESS:
            state.triggerReducer.action = statisticsConstants.TRIGGER_REDUCER_REQUEST_SUCCESS;
            state.triggerReducer.payload = action.payload;
            state.triggerReducer.error = null;
            return;
        case statisticsConstants.TRIGGER_REDUCER_REQUEST_FAILED:
            state.triggerReducer.action = statisticsConstants.TRIGGER_REDUCER_REQUEST_FAILED;
            state.triggerReducer.payload = null;
            state.triggerReducer.error = action.error;
            return;
        default:
            return;
    }
}, initialState);

export default reducer;