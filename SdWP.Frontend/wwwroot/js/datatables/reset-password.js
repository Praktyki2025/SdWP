window.resetPassword = {
    register: function () {
        $('#resetPasswordBtn').on('click', function (e) {
            e.preventDefault();
            const response = await fetch(`/api/user/changepassword`, { method: 'POST' });
            if (!response.ok) throw new Error('Failed to delete project');
        });
    }
};