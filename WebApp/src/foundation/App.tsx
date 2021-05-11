import React from 'react';
import { BrowserRouter,  Route,  Switch } from 'react-router-dom';

import HeaderBar from "./HeaderBar";
import Login from "../features/authentication/Login";
import Registration from "../features/authentication/Registration";
import ConfirmTfa from "../features/authentication/ConfirmTfa";
import ActivateAccount from "../features/authentication/ActivateAccount";
import ForgotPassword from "../features/authentication/ForgotPassword";
import GenericHome from "../features/homepage/GenericHome";

function App() {
    return (
        <>
            <HeaderBar />
            <BrowserRouter>
                <Switch>
                    <Route path='/' exact={ true } component={ Login } />
                    <Route path='/register' component={ Registration } />
                    <Route path='/confirm-tfa' component={ ConfirmTfa } />
                    <Route path='/activate' component={ ActivateAccount } />
                    <Route path='/forgot-password' component={ ForgotPassword } />
                    <Route path='/home' component={ GenericHome } />
                </Switch>
            </BrowserRouter>
        </>
    );
}

export default App;
