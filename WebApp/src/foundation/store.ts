import { createStore, applyMiddleware, compose } from 'redux';
import thunk from 'redux-thunk';
import reducers from '../providers/reducerIndex';
import * as authenticationConstants from '../authentication/redux/constants';

const composeEnhancers = (
    window && (window as any).__REDUX_DEVTOOLS_EXTENSION_COMPOSE__
) || compose;

export const setLocalData = () => (next: any) => (action: any) =>
    {
        if (action.type === authenticationConstants.AUTHENTICATED) {
            localStorage.setItem('authToken', action.payload.authToken);
            localStorage.setItem('userId', action.payload.userId);
        }

        if (action.type === authenticationConstants.NO_AUTHENTICATION) localStorage.clear();
        next(action);
    }

const enhancer = composeEnhancers(
    applyMiddleware(thunk, setLocalData)
);

export const store = createStore(reducers, enhancer);