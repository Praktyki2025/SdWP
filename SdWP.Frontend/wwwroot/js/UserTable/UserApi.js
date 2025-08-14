window.userApi = {
    deleteUser: async function (id) {
        const respone = await fetch(`/api/admin/user/delete/${id}`,
            {
                method: 'DELETE',
            })

        if (!respone.ok) throw new Error(`Error deleting user with ID ${id}: ${respone.statusText}`);
        return respone;
    },

    updateUser: async function (userData) {
        const response = await fetch(`/api/admin/user/update`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(userData)
        });
        if (!response.ok) throw new Error(`Error updating user: ${response.statusText}`);
        return response;
    },

    registerUser: async function (userData) {
        const response = await fetch('/api/admin/user/register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(userData)
        });

        if (!response.ok) throw new Error(`Error registering user: ${response.statusText}`);
        return response;
    }
}