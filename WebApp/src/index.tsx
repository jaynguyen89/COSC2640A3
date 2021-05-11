import React from 'react';
import ReactDOM from 'react-dom';
import App from './foundation/App';
import { Provider } from 'react-redux';
import { store } from './foundation/store';

import { loadAuthUser } from "./features/authentication/redux/actions";
store.dispatch(loadAuthUser());

ReactDOM.render(
    <Provider store={ store }>
        <App />
    </Provider>,
    document.getElementById('root')
);
