export const environment = {
  production: true,
  apiUrl: 'https://YOUR_AZURE_API_URL/api', // Your deployed API URL
  msalConfig: {
    auth: {
      clientId: '8b3cc2bc-c8e1-4406-be1e-14e55cefbe8e',
      authority: 'https://login.microsoftonline.com/67553645-0db3-4480-b127-6f819a79e367',
      redirectUri: 'https://YOUR_FRONTEND_URL',
    },
    cache: {
      cacheLocation: 'localStorage',
      storeAuthStateInCookie: false,
    }
  },
  apiScopes: ['api://8b3cc2bc-c8e1-4406-be1e-14e55cefbe8e/access_as_user']
};
