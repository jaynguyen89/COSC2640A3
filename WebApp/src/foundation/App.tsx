import React from 'react';
import { BrowserRouter,  Route,  Switch } from 'react-router-dom';

import HeaderBar from "./HeaderBar";
import Login from "../authentication/Login";
import Registration from "../authentication/Registration";
import Account from "../account/Account";
import Forum from '../forum/Forum';
import BigQuery from "../bigquery/BigQuery";

function App() {
    return (
        <>
            <HeaderBar />
            <BrowserRouter>
                <Switch>
                    
                </Switch>
            </BrowserRouter>
        </>
    );
}

export default App;
