import React from 'react';
import {IClassroomData} from "../redux/interfaces";
import moment from "moment";

const ClassroomInfo = (props: { classroom: IClassroomData, showTitle?: true }) => {
    return (
        <div className='row'>
            {
                props.showTitle !== undefined &&
                <div className='col s12 center-align'>
                    <h5 style={{ marginTop: 0 }}>Classroom Details</h5>
                    <hr />
                </div>
            }

            <div className='col m6 s12'>
                <span><b>Teacher Name:</b></span>
                <span className='right'>{ props.classroom.teacherName }</span>
            </div>

            {
                props.showTitle !== undefined &&
                <div className='col m6 s12'>
                    <span><b>Classroom Name:</b></span>
                    <span className='right'>{ props.classroom.className }</span>
                </div>
            }

            <div className='col m6 s12'>
                <span><b>Price:</b></span>
                <span className='right'>${ props.classroom.price }</span>
            </div>

            <div className='col m6 s12'>
                <span><b>Places left:</b></span>
                <span className='right'>
                    { props.classroom.classroomDetail.capacity - props.classroom.enrolmentsCount }/{ props.classroom.classroomDetail.capacity }
                </span>
            </div>

            <div className='col m6 s12'>
                <span><b>Duration:</b></span>
                <span className='right'>{ props.classroom.classroomDetail.normalizedDuration }</span>
            </div>

            <div className='col m6 s12'>
                <span><b>Commenced On:</b></span>
                <span className='right'>{ moment(props.classroom.classroomDetail.commencedOn).format('DD MMM YYYY hh:mm') }</span>
            </div>

            <div className='col m6 s12'>
                <span><b>Listed On:</b></span>
                <span className='right'>{ moment(props.classroom.classroomDetail.createdOn).format('DD MMM YYYY hh:mm') }</span>
            </div>
        </div>
    );
}

export default ClassroomInfo;