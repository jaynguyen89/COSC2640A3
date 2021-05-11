import React from 'react';

const HeaderBar = () => {
    return (
        <header className="z-depth-2">
            <div className="container text-center">
                <div className="row">
                    <h4 className="col m6 s12" style={{ margin: 0, lineHeight: '60px' }}>COSC2640 A3</h4>
                    <h4 className="col m6 s12 right-align" style={{ margin: 0, lineHeight: '60px' }}>S3493188 - S3756843</h4>
                </div>
            </div>
        </header>
    );
}

export default HeaderBar;