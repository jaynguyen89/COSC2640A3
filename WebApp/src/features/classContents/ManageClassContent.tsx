import React from 'react';
import { connect } from 'react-redux';
import _ from 'lodash';
import $ from 'jquery';
import M from 'materialize-css';
import Spinner from "../../shared/Spinner";
import Alert from "../../shared/Alert";
import { clearAuthUser } from "../authentication/redux/actions";
import { IClassContent, IFile, IManageClassroomContent, defaultRichContent} from "./redux/interfaces";
import {
    invokeAddFileRequest, invokeUpdateFileRequest,
    invokeAddRichContentRequest, invokeImportRichContentRequest,
    invokeUpdateRichContentRequest, invokeGetClassContent
} from "./redux/actions";
import * as classro from "./redux/constants";
import {
    checkSession,
    EMPTY_STATUS,
    EMPTY_STRING,
    IStatusMessage,
    modalOptions, removeGlobalMessage, setGlobalMessage, TASK_CREATE, TASK_UPDATE,
    TASK_VIEW
} from "../../providers/helpers";
import moment from "moment";
import ClassroomModal from '../classroom/components/ClassroomModal';


const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser,
    addFileClassContent: state.classcontentStore.addFileClassContent,
    updateFileClassContent: state.classcontentStore.updateFileClassContent,
    addRichContent: state.classcontentStore.addRichContent,
    importRichContent: state.classcontentStore.importRichContent,
    updateRichContent: state.classcontentStore.updateRichContent,
    getclassContent: state.classcontentStore.getclassContent
});

const mapActionsToProps = {
    clearAuthUser,
    invokeAddFileRequest,
    invokeUpdateFileRequest,
    invokeAddRichContentRequest,
    invokeImportRichContentRequest,
    invokeUpdateRichContentRequest,
    invokeGetClassContent,
};

const ManageClassContent = (props: IManageClassroomContent) => {
    const [addfiles, setAddFiles] = React.useState(Array<IFile>());
    const [Updatefiles, setUpdateFiles] = React.useState(Array<IFile>());
    const [addRichContent, setAddRichContent] = React.useState(defaultRichContent); 

    React.useEffect(() => {
        props.invokeGetClassContent(props.authUser, null);
        M.Modal.init($('.modal'), {
            ...modalOptions,
            onCloseStart: () => {
                // setSelectedClassroomId(EMPTY_STRING);
                // setSelectedClassroom(defaultClassroom);
                // setIsTaskRunning(false);
                // setModalTask(TASK_VIEW);
                // setStatusMessage(EMPTY_STATUS);
            }
        });
    }, []);



    return (
        <></>
    );
}

export default connect(
    mapStateToProps,
    mapActionsToProps
)(ClassroomModal);