import axios from 'axios';
import { IAuthUser } from "../authentication/redux/interfaces";

//axios.defaults.withCredentials = true; //include all cookies

const LOCAL_ENDPOINT = 'https://localhost:5001/';
const PRODUCTION_ENDPOINT = 'https://cosc2640-307005.ts.r.appspot.com/';

export const sendRequestForResult = (
    action: string,
    auth: IAuthUser | null,
    jsonData: any,
    formData: any = null,
    method: string = 'POST'
) => {
    const authHeaders = auth
        ? {
            'UserId': auth.userId,
            'AuthToken': auth.authToken
        }
        : {};

    const acceptHeaders = { 'Accept': 'application/json' };
    const contentHeaders = formData
        ? { 'Content-Type': 'multipart/form-data' }
        : { 'Content-Type': 'application/json' };

    const headers = {
        ...authHeaders,
        ...contentHeaders,
        ...acceptHeaders
    };

    const body = method === 'POST' || method === 'PUT'
        ? (
            jsonData ? JSON.stringify(jsonData) : formData
        )
        : null;

    const requestOptions : any = {
        timeout: 180000, // 3 minutes
        method: method,
        url: LOCAL_ENDPOINT + action,
        headers: headers,
        data: body
    };

    return axios(requestOptions).then((result: any) => {
        if (result.status !== 200)
            return result.json().then((error: any) => {
                throw error;
            })
        else
            return result.data;
    });
};