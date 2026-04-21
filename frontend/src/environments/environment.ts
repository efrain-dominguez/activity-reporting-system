export const environment = {
  production: false,
  apiUrl: 'https://localhost:7041/api',
  msalConfig: {
    auth: {
      clientId: '8b3cc2bc-c8e1-4406-be1e-14e55cefbe8e', // Replace with your Azure AD Client ID
      authority: 'https://login.microsoftonline.com/67553645-0db3-4480-b127-6f819a79e367', // Replace with your Tenant ID
      redirectUri: 'http://localhost:4200',
    },
    cache: {
      cacheLocation: 'localStorage',
      storeAuthStateInCookie: false,
    }
  },
  apiScopes: ['api://8b3cc2bc-c8e1-4406-be1e-14e55cefbe8e/access_as_user']
};
