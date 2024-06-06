const GOOGLE_CLIENT_ID = import.meta.env.VITE_GOOGLE_CLIENT_ID;
const REMOTE_HOST_NAME = import.meta.env.VITE_BASE_URL;

const APP_ENV = {
    GOOGLE_CLIENT_ID,
    REMOTE_HOST_NAME,
    IMAGE_PATH: `${REMOTE_HOST_NAME}/images/`
};

export { APP_ENV };
