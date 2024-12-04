import React from 'react';
import dynamic from 'next/dynamic';

const DynamicComponentWithNoSSR = dynamic(
    () => import('./Game'),
    {ssr: false}
);

export default () => <DynamicComponentWithNoSSR/>;
