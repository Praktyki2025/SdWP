window.projectsApi = {
    deleteProject: async function (id) {
        const deleteRequest = {
            id: id,
            //reason: reason || null // for example reason for deletion
        };

        const response = await fetch(`/api/projects/delete`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(deleteRequest)
        });

        if (!response.ok) {
            throw new Error(`Failed to delete project: ${response.status}`);
        }

        return response.json();
    }
};
