import React from 'react';
import {connect} from "react-redux";
import moment from "moment";
import {defaultProgress, IEmrProgress, IEmrStatistics, IStatistics} from "./redux/interfaces";
import {invokeGetStatisticsRequest, invokeTriggerMapperRequest, invokeTriggerReducerRequest} from "./redux/actions";
import * as statisticsConstants from './redux/constants';
import {EMPTY_STATUS, IStatusMessage} from "../../providers/helpers";
import Spinner from "../../shared/Spinner";
import Alert from "../../shared/Alert";

const mapStateToProps = (state: any) => ({
    getStatistics: state.statisticsStore.getStatistics,
    triggerMapper: state.statisticsStore.triggerMapper,
    triggerReducer: state.statisticsStore.triggerReducer,
});

const mapActionsToProps = {
    invokeGetStatisticsRequest,
    invokeTriggerMapperRequest,
    invokeTriggerReducerRequest
};

const Statistics = (props: IStatistics) => {
    const [progress, setProgress] = React.useState(defaultProgress);
    const [statistics, setStatistics] = React.useState(Array<IEmrStatistics>());
    const [statusMessage, setStatusMessage] = React.useState(EMPTY_STATUS);

    const [shouldEnableMapperButton, setShouldEnableMapperButton] = React.useState(false);
    const [shouldEnableReducerButton, setShouldEnableReducerButton] = React.useState(false);

    React.useEffect(() => {
        props.invokeGetStatisticsRequest();
    }, []);

    React.useEffect(() => {
        if (props.getStatistics.action === statisticsConstants.GET_STATISTICS_RESULTS_REQUEST_FAILED)
            setStatusMessage({ messages: ['Unable to communicate with server: connection timed out.'], type: 'error' } as IStatusMessage);

        if (props.getStatistics.action === statisticsConstants.GET_STATISTICS_RESULTS_REQUEST_SUCCESS)
            if (props.getStatistics.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.getStatistics.payload.result === 0)
                setStatusMessage({ messages: props.getStatistics.payload.messages, type: 'error' } as IStatusMessage);
            else {
                setProgress((props.getStatistics.payload.data as any).progress as IEmrProgress);
                setStatistics((props.getStatistics.payload.data as any).statistics as Array<IEmrStatistics>);
            }
    }, [props.getStatistics]);

    React.useEffect(() => {
        setShouldEnableMapperButton(progress.mapperDone && progress.reducerDone);
        setShouldEnableReducerButton(progress.mapperDone);
    }, [progress]);

    const attemptTriggeringMapper = () => props.invokeTriggerMapperRequest();
    const attemptTriggeringReducer = () => props.invokeTriggerReducerRequest();

    React.useEffect(() => {
        if (props.triggerMapper.action === statisticsConstants.TRIGGER_MAPPER_REQUEST_FAILED)
            setStatusMessage({ messages: ['Unable to communicate with server: connection timed out.'], type: 'error' } as IStatusMessage);

        if (props.triggerMapper.action === statisticsConstants.TRIGGER_MAPPER_REQUEST_SUCCESS)
            if (props.triggerMapper.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.triggerMapper.payload.result === 0)
                setStatusMessage({ messages: props.triggerMapper.payload.messages, type: 'error' } as IStatusMessage);
            else
                props.invokeGetStatisticsRequest();
    }, [props.triggerMapper]);

    React.useEffect(() => {
        if (props.triggerReducer.action === statisticsConstants.TRIGGER_REDUCER_REQUEST_FAILED)
            setStatusMessage({ messages: ['Unable to communicate with server: connection timed out.'], type: 'error' } as IStatusMessage);

        if (props.triggerReducer.action === statisticsConstants.TRIGGER_REDUCER_REQUEST_SUCCESS)
            if (props.triggerReducer.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.triggerReducer.payload.result === 0)
                setStatusMessage({ messages: props.triggerReducer.payload.messages, type: 'error' } as IStatusMessage);
            else
                props.invokeGetStatisticsRequest();
    }, [props.triggerReducer]);

    return (
        <div className='main-content'>
            <div className='row'>
                <div className='col s12 center-align'>
                    <h4 style={{ marginTop: 0 }}>Map-Reduce Control Board</h4>
                    <hr />
                </div>

                <div className='col s12'>
                    {
                        (props.getStatistics.action === statisticsConstants.GET_STATISTICS_RESULTS_REQUEST_SENT ||
                        props.triggerMapper.action === statisticsConstants.TRIGGER_MAPPER_REQUEST_SENT ||
                        props.triggerReducer.action === statisticsConstants.TRIGGER_REDUCER_REQUEST_SENT) && <Spinner />
                    }
                    <Alert { ...statusMessage } closeAlert={ () => setStatusMessage(EMPTY_STATUS) } />

                    <div className='card' style={{ width: '50%', margin: '0 auto' }}>
                        <div className='card-content grey lighten-3'>
                            <p><i className="fas fa-angle-right" /> Job ID: { progress.id }</p>
                            <p><i className="fas fa-angle-right" /> Mapper progress: { (progress.mapperDone && 'FINISHED') || 'Processing data...' }</p>
                            <p><i className="fas fa-angle-right" /> Reducer progress: { (progress.reducerDone && 'FINISHED') || 'Awaiting progress...' }</p>
                            <p><i className="fas fa-angle-right" /> Last updated: { moment(progress.timestamp).format('DD MMM yyyy HH:mm:ss') } - ({ moment(progress.timestamp).fromNow() })</p>
                        </div>
                        <div className='card-action'>
                            <button className={ (shouldEnableMapperButton && 'btn waves-effect waves-light') || 'btn disabled' }
                                    onClick={ () => attemptTriggeringMapper() }
                            >
                                <i className="fas fa-bolt" />&nbsp;
                                Trigger Mapper
                            </button>

                            <button className={ (shouldEnableReducerButton && 'btn waves-effect waves-light right') || 'btn disabled' }
                                    onClick={ () => attemptTriggeringReducer() }
                            >
                                <i className="fas fa-bolt" />&nbsp;
                                Trigger Reducer
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            <div className='row'>
                <div className='col s12 center-align'>
                    <h4 style={{ marginTop: 0 }}>Statistics</h4>
                    <hr />
                </div>

                <div className='col s12'>
                    {
                        (
                            statistics.length === 0 &&
                            <p className='center-align' style={{ width: '50%', margin: 'auto' }}>
                                No statistics have been processed. Please trigger Map-Reduce job to process statistics.
                            </p>
                        ) ||
                        <div className='row'>
                            {
                                statistics.map(statistic =>
                                    <div className='col m4 s6' key={ statistic.id }>
                                        <div className='card'>
                                            <div className='card-content'>
                                                <p>Range $50 & below: <span className='right'>{ statistic.range50 }%</span></p>
                                                <p>Range $50 - $100: <span className='right'>{ statistic.range100 }%</span></p>
                                                <p>Range $100 - $250: <span className='right'>{ statistic.range250 }%</span></p>
                                                <p>Range $250 - $500: <span className='right'>{ statistic.range500 }%</span></p>
                                                <p>Range $500 - $1000: <span className='right'>{ statistic.range1000 }%</span></p>
                                                <p>Range over $1000: <span className='right'>{ statistic.range1001 }%</span></p>
                                                <p>Processed On: <span className='right'>{ moment(statistic.timestamp).format('DD MMM yyyy HH:mm:ss') }</span></p>
                                            </div>
                                        </div>
                                    </div>
                                )
                            }
                        </div>
                    }
                </div>
            </div>
        </div>
    );
}

export default connect(
    mapStateToProps,
    mapActionsToProps
)(Statistics);