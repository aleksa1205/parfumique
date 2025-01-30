export const HttpMethods = {
    GET: { label: 'GET', className: 'bg-blue-200 text-blue-700' },
    POST: { label: 'POST', className: 'bg-green-200 text-green-700' },
    PUT: { label: 'PUT', className: 'bg-yellow-300 text-yellow-800' },
    PATCH: { label: 'PATCH', className: 'bg-brand-300 text-brand-800' },
    DELETE: { label: 'DELETE', className: 'bg-red-200 text-red-700' },
} as const;

export const EndpointRequirements = {
    Authorization: { label: 'Authorization', className: 'border-blue-200 text-blue-800 group-hover:bg-blue-200'},
    Admin: { label: 'Admin Role', className: 'border-yellow-300 text-yellow-800 group-hover:bg-yellow-300'},
    User: { label: 'User Role', className: 'border-brand-300 text-brand-800 group-hover:bg-brand-300'},
} as const;

export type EndpointRequirementsArray = (keyof typeof EndpointRequirements)[]