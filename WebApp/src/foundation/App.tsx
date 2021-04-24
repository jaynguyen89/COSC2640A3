import React from 'react';
import { BrowserRouter,  Route,  Switch } from 'react-router-dom';

import HeaderBar from "./HeaderBar";
import Login from "../features/authentication/Login";
import Registration from "../features/authentication/Registration";

function App() {
    return (
        <>
            <HeaderBar />
            <BrowserRouter>
                <Switch>
                    <Route path='/' exact={ true } component={ Login } />
                    <Route path='/register' component={ Registration } />
                </Switch>
            </BrowserRouter>
        </>
    );
}

export default App;
