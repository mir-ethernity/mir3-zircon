import 'reflect-metadata';
import React from 'react';
import ReactDOM from 'react-dom';
import App from './App';
import { BrowserRouter } from 'react-router-dom';
import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import localeEN from './locale/en.json';
import config from 'config';
import * as Sentry from '@sentry/browser';
import './index.scss';

if (config.REACT_APP_SENTRY_ENABLED) {
  Sentry.init({ dsn: config.REACT_APP_SENTRY_DNS, release: config.REACT_APP_VERSION });
}


const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
const rootElement = document.getElementById('root');

i18n
  .use(initReactI18next)
  .init({
    fallbackLng: 'en',
    debug: false,
    resources: {
      en: localeEN
    },
    whitelist: ['en']
  });

ReactDOM.render(
  <BrowserRouter basename={baseUrl || ''}>
    <App />
  </BrowserRouter>,
  rootElement);


