import React from 'react';
import {IClassroomData} from "../../classroom/redux/interfaces";
import moment from "moment";
import {TimeUnits} from "../../../providers/helpers";

const ClassroomSummary = (props: { classroom: IClassroomData }) => {
    return (
        <div className='row' style={{ margin: 0, borderLeft: '1px solid #50a8c0', paddingLeft: '0.5em' }}>
            <h5 className='title'>
                <i className="fas fa-align-center" />
                &nbsp;Classroom Summary
            </h5>

            <div className='col m12 s6' style={{ padding: 0, marginBottom: '0.5em' }}>
                <p className='small-text' style={{ margin: 0 }}>Teacher:</p>
                <p style={{ margin: 0 }} className='right'>{ props.classroom.teacherName }</p>
            </div>
            <div className='col m12 s6' style={{ padding: 0, marginBottom: '0.5em' }}>
                <p className='small-text' style={{ margin: 0 }}>Duration:</p>
                <p style={{ margin: 0 }} className='right'>{ props.classroom.classroomDetail.normalizedDuration }</p>
            </div>
            <div className='col m12 s6' style={{ padding: 0, marginBottom: '0.5em' }}>
                <p className='small-text' style={{ margin: 0 }}>Created On:</p>
                <p style={{ margin: 0 }} className='right'>{ moment(props.classroom.classroomDetail.createdOn).format('DD MMM YYYY hh:mm') }</p>
            </div>
            <div className='col m12 s6' style={{ padding: 0, marginBottom: '0.5em' }}>
                <p className='small-text' style={{ margin: 0 }}>Commence On:</p>
                <p style={{ margin: 0 }} className='right'>{ moment(props.classroom.classroomDetail.commencedOn).format('DD MMM YYYY hh:mm') }</p>
            </div>
            <div className='col m12 s6' style={{ padding: 0, marginBottom: '0.5em' }}>
                <p className='small-text' style={{ margin: 0 }}>Finished On (provisioned):</p>
                <p style={{ margin: 0 }} className='right'>
                    {
                        moment(props.classroom.classroomDetail.commencedOn)
                            .add(props.classroom.classroomDetail.duration, TimeUnits[props.classroom.classroomDetail.durationUnit] as moment.unitOfTime.DurationConstructor)
                            .format('DD MMM YYYY hh:mm')
                    }
                </p>
            </div>
            <div className='col m12 s6' style={{ padding: 0, marginBottom: '0.5em' }}>
                <p className='small-text' style={{ margin: 0 }}>Capacity:</p>
                <p style={{ margin: 0 }} className='right'>{ props.classroom.classroomDetail.capacity } students</p>
            </div>
            <div className='col m12 s6' style={{ padding: 0, marginBottom: '0.5em' }}>
                <p className='small-text' style={{ margin: 0 }}>Enrolments:</p>
                <p style={{ margin: 0 }} className='right'>{ (props.classroom.enrolmentsCount && props.classroom.enrolmentsCount + ' students ') || 'N/A' }</p>
            </div>
        </div>
    );
}

export default ClassroomSummary;