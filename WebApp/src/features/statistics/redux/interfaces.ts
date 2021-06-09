import {EMPTY_STRING, IActionResult} from "../../../providers/helpers";

export interface IStatistics {
    getStatistics: IActionResult,
    triggerMapper: IActionResult,
    triggerReducer: IActionResult,
    invokeGetStatisticsRequest: () => void,
    invokeTriggerMapperRequest: () => void,
    invokeTriggerReducerRequest: () => void
}

export interface IEmrProgress {
    id: string,
    timestamp: string,
    mapperDone: boolean,
    reducerDone: boolean
}

export const defaultProgress: IEmrProgress = {
    id: EMPTY_STRING,
    timestamp: EMPTY_STRING,
    mapperDone: false,
    reducerDone: false
}

export interface IEmrStatistics {
    id: string,
    timestamp: string,
    range50: number,
    range100: number,
    range250: number,
    range500: number,
    range1000: number,
    range1001: number
}