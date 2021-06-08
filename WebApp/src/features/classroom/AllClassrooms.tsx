import React from 'react';
import {connect} from 'react-redux';
import $ from 'jquery';
import M from 'materialize-css';
import Spinner from "../../shared/Spinner";
import Alert from "../../shared/Alert";
import HeaderNav from "../../shared/HeaderNav";
import {CLASSROOMS_PER_PAGE, defaultClassroom, IAllClassrooms, IClassroomData} from "./redux/interfaces";
import {clearAuthUser} from "../authentication/redux/actions";
import {
    invokeGetAllClassroomsRequest,
    invokeGetClassroomDetailRequest,
    invokeSearchClassroomsRequest
} from "./redux/actions";
import {
    checkSession,
    EMPTY_STATUS,
    EMPTY_STRING, isProperString,
    IStatusMessage,
    modalOptions, removeGlobalMessage, setGlobalMessage
} from "../../providers/helpers";
import * as classroomConstants from './redux/constants';
import * as studentConstants from '../student/redux/constants';
import {invokeStudentEnrolmentRequest} from "../student/redux/actions";
import ClassroomInfo from "./components/ClassroomInfo";
import Pagination from "./components/Pagination";

const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser,
    getAllClassrooms: state.classroomStore.getAllClassrooms,
    getClassroomDetail: state.classroomStore.getClassroomDetail,
    enrolClassroom: state.studentStore.enrolClassroom,
    searchClassrooms: state.classroomStore.searchClassrooms
});

const mapActionsToProps = {
    clearAuthUser,
    invokeGetAllClassroomsRequest,
    invokeGetClassroomDetailRequest,
    invokeStudentEnrolmentRequest,
    invokeSearchClassroomsRequest
};

const AllClassrooms = (props: IAllClassrooms) => {
    const [classrooms, setClassrooms] = React.useState(Array<IClassroomData>());
    const [paging, setPaging] = React.useState({ offset: 0, isLast: false });
    const [searchData, setSearchData] = React.useState({ classroomName: EMPTY_STRING, teacherName: EMPTY_STRING });
    const [shouldShowSearchResult, setShouldShowSearchResult] = React.useState(false);
    const [searchResults, setSearchResults] = React.useState(Array<IClassroomData>());
    const [selectedClassroomId, setSelectedClassroomId] = React.useState(EMPTY_STRING);
    const [classroomDetails, setClassroomDetails] = React.useState(defaultClassroom);
    const [statusMessage, setStatusMessage] = React.useState(EMPTY_STATUS);
    const [shouldEnableButtons, setShouldEnableButtons] = React.useState(true);

    React.useEffect(() => {
        const storedGlobalMessage = sessionStorage.getItem('globalMessage');
        if (storedGlobalMessage) {
            setStatusMessage(JSON.parse(storedGlobalMessage) as IStatusMessage);
            removeGlobalMessage();
        }

        M.Modal.init($('.modal'), {
            ...modalOptions,
            onCloseStart: () => {
                setShouldEnableButtons(true);
                setSelectedClassroomId(EMPTY_STRING);
                setClassroomDetails(defaultClassroom);
            }
        });

        props.invokeGetAllClassroomsRequest(props.authUser, 0);
    }, []);

    React.useEffect(() => {
        if (props.getAllClassrooms.action === classroomConstants.GET_ALL_CLASSROOMS_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.getAllClassrooms.error?.message);

        if (props.getAllClassrooms.action === classroomConstants.GET_ALL_CLASSROOMS_REQUEST_SUCCESS)
            if (props.getAllClassrooms.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.getAllClassrooms.payload.result === 0)
                setStatusMessage({ messages: props.getAllClassrooms.payload.messages, type: 'error' } as IStatusMessage);
            else {
                setClassrooms((props.getAllClassrooms.payload.data as any).classrooms as Array<IClassroomData>);
                setPaging({ ...paging, isLast: (props.getAllClassrooms.payload.data as any).isLast });
            }
    }, [props.getAllClassrooms]);

    const updateSearchData = (field: string, val: string) => {
        if (field === 'teacher') setSearchData({ ...searchData, teacherName: val });
        if (field === 'classroom') setSearchData({ ...searchData, classroomName: val });
    }

    const attemptSearchClassrooms = () => {
        if (!isProperString(searchData.classroomName) && !isProperString(searchData.teacherName)) {
            alert('No classroom name or teacher name was entered. No search will be done.');
            return;
        }

        setShouldShowSearchResult(false);
        props.invokeSearchClassroomsRequest(props.authUser, searchData);
    }

    const loadMoreClassrooms = (next: boolean | null) => {
        let currentOffset = paging.offset;
        if (next === null) {
            props.invokeGetAllClassroomsRequest(props.authUser, 0);
            setPaging({ ...paging, offset: 0 });
            return;
        }

        if (next) {
            props.invokeGetAllClassroomsRequest(props.authUser, currentOffset + CLASSROOMS_PER_PAGE);
            setPaging({ ...paging, offset: currentOffset + CLASSROOMS_PER_PAGE });
            return;
        }

        props.invokeGetAllClassroomsRequest(props.authUser, currentOffset - CLASSROOMS_PER_PAGE);
        setPaging({ ...paging, offset: currentOffset - CLASSROOMS_PER_PAGE });
    }

    const handleViewingDetails = (classroomId: string) => {
        setShouldEnableButtons(false);
        setSelectedClassroomId(classroomId);
        props.invokeGetClassroomDetailRequest(props.authUser, classroomId);
    }

    const attemptEnrolment = (classroomId: string) => {
        setSelectedClassroomId(classroomId);
        props.invokeStudentEnrolmentRequest(props.authUser, classroomId);
    }

    React.useEffect(() => {
        setSelectedClassroomId(EMPTY_STRING);

        if (props.getClassroomDetail.action === classroomConstants.GET_CLASSROOM_DETAILS_REQUEST_SENT)
            checkSession(props.clearAuthUser, setStatusMessage, props.getClassroomDetail.error?.message);

        if (props.getClassroomDetail.action === classroomConstants.GET_CLASSROOM_DETAILS_REQUEST_SUCCESS)
            if (props.getClassroomDetail.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.getClassroomDetail.payload.result === 0)
                setStatusMessage({ messages: props.getClassroomDetail.payload.messages, type: 'error' } as IStatusMessage);
            else {
                setStatusMessage(EMPTY_STATUS);
                setClassroomDetails(props.getClassroomDetail.payload.data as IClassroomData);
                M.Modal.getInstance(document.querySelector('#classroomDetailModal') as Element).open();
            }
    }, [props.getClassroomDetail]);

    React.useEffect(() => {
        setSelectedClassroomId(EMPTY_STRING);

        if (props.enrolClassroom.action === studentConstants.ENROL_INTO_CLASSROOM_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.enrolClassroom.error?.message);

        if (props.enrolClassroom.action === studentConstants.ENROL_INTO_CLASSROOM_REQUEST_SUCCESS)
            if (props.enrolClassroom.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.enrolClassroom.payload.result === 0)
                setStatusMessage({ messages: props.enrolClassroom.payload.messages, type: 'error' } as IStatusMessage);
            else {
                setStatusMessage(EMPTY_STATUS);
                setGlobalMessage({ messages: ['You have successfully enrolled into the classroom, view it in your home page.'], type: 'success' } as IStatusMessage);
                props.invokeGetAllClassroomsRequest(props.authUser, paging.offset > CLASSROOMS_PER_PAGE ? paging.offset - CLASSROOMS_PER_PAGE : 0);
            }
    }, [props.enrolClassroom]);

    React.useEffect(() => {
        if (props.searchClassrooms.action === classroomConstants.SEARCH_CLASSROOMS_REQUEST_FAILED)
            checkSession(props.clearAuthUser, setStatusMessage, props.searchClassrooms.error?.message);

        if (props.searchClassrooms.action === classroomConstants.SEARCH_CLASSROOMS_REQUEST_SUCCESS)
            if (props.searchClassrooms.payload === null)
                setStatusMessage({ messages: ['Failed to send request to server. Please try again.'], type: 'error' } as IStatusMessage);
            else if (props.searchClassrooms.payload.result === 0)
                setStatusMessage({ messages: props.searchClassrooms.payload.messages, type: 'error' } as IStatusMessage);
            else {
                setShouldShowSearchResult(true);
                setSearchResults(props.searchClassrooms.payload.data as Array<IClassroomData>);
            }
    }, [props.searchClassrooms]);

    return (
        <div className='container' style={{ marginTop: '2.5em', marginBottom: '4em' }}>
            <HeaderNav
                location='home'
                greetingName={ localStorage.getItem('preferredName') as string }
            />
            { props.getAllClassrooms.action === classroomConstants.GET_ALL_CLASSROOMS_REQUEST_SENT && <Spinner /> }
            <Alert { ...statusMessage } closeAlert={ () => setStatusMessage(EMPTY_STATUS) } />

            <div className='row'>
                <div className='col s12 center-align'>
                    <h4 style={{ marginTop: 0 }}>Browsing Classrooms</h4>
                    <hr />
                </div>

                <div className='col s12'>
                    <div className='card'>
                        <div className='row' style={{ padding: '1em' }}>
                            <div className='col s5'>
                                <div className="input-field" style={{ margin: 0 }}>
                                    <input id="classroomName" type="text"
                                           value={ searchData.classroomName }
                                           onChange={ e => updateSearchData('classroom', e.target.value) }
                                    />
                                    <label htmlFor="classroomName">Search by classroom's name</label>
                                </div>
                            </div>
                            <div className='col s5'>
                                <div className="input-field" style={{ margin: 0 }}>
                                    <input id="teacherName" type="text"
                                           value={ searchData.teacherName }
                                           onChange={ e => updateSearchData('teacher', e.target.value) }
                                    />
                                    <label htmlFor="teacherName">Search by teacher's name</label>
                                </div>
                            </div>
                            <div className='col s2'>
                                <button className='btn waves-effect waves-light'
                                        onClick={ () => attemptSearchClassrooms() }
                                >
                                    <i className='fas fa-search' />&nbsp;
                                    Search
                                </button>
                            </div>
                        </div>
                    </div>

                    {
                        shouldShowSearchResult &&
                        <div className='row'>
                            <div className='col s12'>
                                <div className='card' style={{ maxHeight: '35em', overflowY: 'scroll' }}>
                                    <i className='fas fa-times corner right-align text-link red-text'
                                       onClick={ () => {
                                           setSearchResults(Array<IClassroomData>());
                                           setShouldShowSearchResult(false);
                                           setSearchData({ classroomName: EMPTY_STRING, teacherName: EMPTY_STRING });
                                       }}
                                    />

                                    <div className='card-content'>
                                        <table className='responsive-table striped'>
                                            <thead>
                                            <tr>
                                                <td><b>Classroom Name</b></td>
                                                <td><b>Teacher Name</b></td>
                                                <td><b>Price</b></td>
                                                <td><b>Actions</b></td>
                                            </tr>
                                            </thead>
                                            <tbody>
                                            {
                                                (
                                                    searchResults.length === 0 &&
                                                    <tr>
                                                        <td className='center-align' colSpan={ 4 }>No classroom matches your search inputs. Please try again with fewer details.</td>
                                                    </tr>
                                                ) ||
                                                searchResults.map(result =>
                                                    <tr>
                                                        <td>{ result.className }</td>
                                                        <td>{ result.teacherName }</td>
                                                        <td>${ result.price }</td>
                                                        <td>
                                                            <button className={ (shouldEnableButtons && 'btn waves-effect waves-light') || 'btn disabled' }
                                                                    onClick={ () => handleViewingDetails(result.id) }
                                                            >
                                                                <i className="fas fa-align-center" />&nbsp;
                                                                Details
                                                            </button>

                                                            <button className={ (shouldEnableButtons && 'btn waves-effect waves-light orange') || 'btn disabled' }
                                                                    onClick={ () => attemptEnrolment(result.id) }
                                                                    style={{ marginLeft: '1em' }}
                                                            >
                                                                <i className="fas fa-sign-in-alt" />&nbsp;
                                                                Enrol
                                                            </button>
                                                        </td>
                                                    </tr>
                                                )
                                            }
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </div>
                    }
                </div>

                <div className='col s12 center-align' style={{ marginBottom: '1em' }}>
                    <Pagination paging={ paging } clickHandler={ loadMoreClassrooms } />
                </div>

                {
                    classrooms.map(classroom =>
                        <div className='col m4 s12' key={ classroom.id }>
                            <div className='card'>
                                <div className='row card-content'>
                                    {
                                        (
                                            props.getClassroomDetail.action === classroomConstants.GET_CLASSROOM_DETAILS_REQUEST_SENT ||
                                            props.enrolClassroom.action === studentConstants.ENROL_INTO_CLASSROOM_REQUEST_SENT
                                        ) &&
                                        selectedClassroomId === classroom.id &&
                                        <div className='corner'><Spinner /></div>
                                    }

                                    <a className='section-header text-link'
                                       onClick={ () => handleViewingDetails(classroom.id) }
                                    >
                                        <i className="fas fa-link" />
                                        &nbsp;{ classroom.className }
                                    </a>


                                    <div className='col s12 small-text'>
                                        <span>Teacher:</span>
                                        <span className='right'>{ classroom.teacherName }</span>
                                    </div>

                                    <div className='col s12 small-text'>
                                        <span>Price:</span>
                                        <span className='right'>${ classroom.price }</span>
                                    </div>

                                    <div className='col s12 small-text' style={{ marginTop: '1em' }}>
                                        <button className={ (shouldEnableButtons && 'btn waves-effect waves-light') || 'btn disabled' }
                                                onClick={ () => handleViewingDetails(classroom.id) }
                                        >
                                            <i className="fas fa-align-center" />&nbsp;
                                            Details
                                        </button>

                                        <button className={ (shouldEnableButtons && 'btn waves-effect waves-light orange right') || 'btn disabled right' }
                                                onClick={ () => attemptEnrolment(classroom.id) }
                                        >
                                            <i className="fas fa-sign-in-alt" />&nbsp;
                                            Enrol
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    )
                }

                <div className='col s12 center-align' style={{ marginTop: '1.5em' }}>
                    <Pagination paging={ paging } clickHandler={ loadMoreClassrooms } />
                </div>
            </div>

            <div className='modal' id='classroomDetailModal'>
                <ClassroomInfo classroom={ classroomDetails } showTitle />
            </div>
        </div>
    );
}

export default connect(
    mapStateToProps,
    mapActionsToProps
)(AllClassrooms);