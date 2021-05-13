import React from 'react';
import {connect} from 'react-redux';
import {IClassroom, IClassroomCard} from "../redux/interfaces";

const mapStateToProps = (state: any) => ({
    authUser: state.authenticationStore.authUser
});

const mapActionsToProps = {};

const ClassroomCard = (props: IClassroomCard) => {
    return (
        <div className='card'>
            <div className='card-content'>
                <a className='section-header text-link'
                   onClick={ () => props.handleTitleClicked(props.classroom.id) }
                >
                    <i className="fas fa-link" />
                    &nbsp;{ props.classroom.className }
                </a>

                {
                    (
                        props.completed === undefined &&
                        <table>
                            <thead>
                            <tr>
                                <th>Price</th>
                                <th>Enrolments</th>
                            </tr>
                            </thead>
                            <tbody>
                            <tr>
                                <td>${ props.classroom.price }</td>
                                <td>{ props.classroom.enrolmentsCount }</td>
                            </tr>
                            </tbody>
                        </table>
                    ) ||
                    <p className=''>
                        <b>Income: </b>
                        { props.classroom.enrolmentsCount } x ${ props.classroom.price } = ${ props.classroom.enrolmentsCount * props.classroom.price }
                    </p>
                }
            </div>

            {
                props.completed === undefined &&
                <div className='card-action'>
                    <a className='small-text text-link teal-text'
                       onClick={ () => props.handleReviseBtn && props.handleReviseBtn(props.classroom.id) }
                    >
                        Revise
                    </a>

                    <a className='small-text text-link amber-text'
                       onClick={ () => props.handleMarkAsCompletedBtn && props.handleMarkAsCompletedBtn(props.classroom.id) }
                    >
                        Mark as Completed
                    </a>

                    <a className='small-text text-link red-text'
                       onClick={ () => props.handleRemoveBtn && props.handleRemoveBtn(props.classroom.id) }
                    >
                        Remove
                    </a>
                </div>
            }
        </div>
    );
}

export default connect(
    mapStateToProps,
    mapActionsToProps
)(ClassroomCard);