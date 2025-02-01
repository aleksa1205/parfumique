export default function AdminDashboard() {
    return (
        <main className="max-w-screen-xl mx-auto px-4 [&>h2]:text-3xl [&>h2]:font-bold [&>h2]:my-4">
            <br />
            <h2>Welcome to the Admin Dashboard</h2>
            <br />

            <ul>This dashboard provides you with access to various controllers in the server, each represented by a sublink. Here’s an overview of what you can do: <br/><br/>
                <li>• Fragrance Controller: Access fragrance data. This allows you to manage and view all fragrance-related information stored in the system.</li>
                <li>• Note Controller: View and manage fragrance notes. Notes represent the various scent categories (top, middle, and base) used to describe the composition of fragrances.</li>
                <li>• Manufacturer Controller: Manage manufacturers of fragrances. You can view and update manufacturer information.</li>
                <li>• Perfumer Controller: Manage and view information about perfumers. This includes details about the people behind the fragrance creations.</li>
                <li>• User Controller: View and manage user data. This allows you to see and update information about users who interact with the system.</li>
            </ul>

            <br />
            <p>
            Each of these controllers has its own dedicated endpoint that the dashboard links to. For now, these endpoints either require no special role or an Admin role to access. The admin role grants you full access to manage and view all data.
            </p>
            <br />

            <h2>End Points & Data Handling</h2>
            <br /><p>• The endpoints are designed to handle raw input and return raw output at the moment. This means you'll be working directly with the data without any additional formatting or validation.</p>
            <br /><p>• No Role: Some endpoints are accessible by anyone who has the link.</p>
           <br /> <p>• Admin Role: Other endpoints require the admin role to access, allowing you to make changes and view sensitive data.</p>

            <br />
            <h2>Future Enhancements</h2><br />
            <p>In future versions, we’ll be improving the system:</p>

            <p><br />• Input Validation: Proper validation will be added to ensure that all data entered into the system is accurate and follows the correct format.</p>
            <p><br />• Better UI: A more user-friendly and intuitive interface will be introduced to make your experience more efficient and pleasant.</p>

            <p><br />For now, please feel free to use the current features and let us know if you have any feedback or suggestions!</p>
        </main>
    )
}