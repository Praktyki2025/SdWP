window.apiLogin = async (loginModel) => {
    try {
        const loginResponse = await fetch('/api/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(loginModel)
        });

        return await loginResponse.json();

    } catch (error) {
        console.error('Login API process failed:', error);
        return { success: false, message: 'A critical error occurred during login.' };
    }
};

window.apiLogout = async function () {
    const response = await fetch('/api/login/logout', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        credentials: 'include'
    });

    if (response.ok) {
        window.location.href = '/login';
    } else {
        console.error('Logout failed:', response.statusText);
    }
};