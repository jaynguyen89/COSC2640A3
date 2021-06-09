import * as statisticsConstants from './constants';
import * as statisticsServices from './services';

export const invokeGetStatisticsRequest = () => {
    return (dispatch: any) => {
        dispatch({ type: statisticsConstants.GET_STATISTICS_RESULTS_REQUEST_SENT });

        statisticsServices.sendGetStatisticsRequest()
            .then(response => dispatch({
                type: statisticsConstants.GET_STATISTICS_RESULTS_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: statisticsConstants.GET_STATISTICS_RESULTS_REQUEST_FAILED,
                error
            }))
    };
}

export const invokeTriggerMapperRequest = () => {
    return (dispatch: any) => {
        dispatch({ type: statisticsConstants.TRIGGER_MAPPER_REQUEST_SENT });

        statisticsServices.sendTriggerMapperRequest()
            .then(response => dispatch({
                type: statisticsConstants.TRIGGER_MAPPER_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: statisticsConstants.TRIGGER_MAPPER_REQUEST_FAILED,
                error
            }))
    };
}

export const invokeTriggerReducerRequest = () => {
    return (dispatch: any) => {
        dispatch({ type: statisticsConstants.TRIGGER_REDUCER_REQUEST_SENT });

        statisticsServices.sendTriggerReducerRequest()
            .then(response => dispatch({
                type: statisticsConstants.TRIGGER_REDUCER_REQUEST_SUCCESS,
                payload: response
            }))
            .catch(error => dispatch({
                type: statisticsConstants.TRIGGER_REDUCER_REQUEST_FAILED,
                error
            }))
    };
}