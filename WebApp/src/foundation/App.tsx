import React from 'react';
import { BrowserRouter,  Route,  Switch } from 'react-router-dom';

import HeaderBar from "./HeaderBar";
import Login from "../features/authentication/Login";
import Registration from "../features/authentication/Registration";
import ConfirmTfa from "../features/authentication/ConfirmTfa";
import ActivateAccount from "../features/authentication/ActivateAccount";
import ForgotPassword from "../features/authentication/ForgotPassword";
import GenericHome from "../features/homepage/GenericHome";
import ManageClassrooms from "../features/classroom/ManageClassrooms";
import ManageClassContent from "../features/classContents/ManageClassContent";

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
                    <Route path='/manage-classrooms' component={ ManageClassrooms } />
                    <Route path='/manage-classroom-contents' component={ ManageClassContent } />
                </Switch>
            </BrowserRouter>
        </>
    );
}

export default App;
