import { Link } from "react-router-dom";
import { AdminPages } from "../../layers/AdminRequiredLayer"

type PropValues = {
    activeAdminPage: AdminPages,
}

export default function AdminFirstBlock({ activeAdminPage}: PropValues) {
    let activePageComponent = null;
    if (activeAdminPage === AdminPages.Dashboard)
            activePageComponent = <span className="font-semibold text-brand-600"> Admin Dashboard</span>
    else {
        activePageComponent = (
            <>
                <span> Admin Dashboard / </span>
                <span className="font-semibold text-brand-600"> {AdminPages[activeAdminPage]}</span>
            </>
        )
    }

    return (
        <div className="max-w-screen-xl mx-auto px-4 mt-20">
            <h3 className="text-neutral-400 my-4">
                Home / 
                {activePageComponent}
            </h3>

            <h2 className="font-bold my-4 text-3xl">
                {AdminPages[activeAdminPage]}
            </h2>

            <AdminPagesButtonGroup activeAdminPage={activeAdminPage} />
        </div>
    )
}

type AdminPagesButtonGroupProps = {
    activeAdminPage: AdminPages;
}

function AdminPagesButtonGroup({activeAdminPage} : AdminPagesButtonGroupProps) {
    const adminPagesArray = Object.values(AdminPages)
                            .filter((value): value is keyof typeof AdminPages => typeof value === "string");
    return (
        <div className="max-w-max">
            <ul className="flex gap-x-3 bg-neutral-50 py-1.5 px-2.5 rounded-md font-medium">
                {adminPagesArray.map(value => {
                    return (
                        <AdminPagesButton 
                            key={value} 
                            text={value} 
                            activeAdminPage={activeAdminPage}
                            linkTo={`/admin-dashboard/${value === AdminPages[AdminPages.Dashboard]
                                    ? ''
                                    : value
                            }`}
                        />
                    )
                })}
            </ul>
        </div>
    )
}

type AdminPagesButtonProps = {
    text: keyof typeof AdminPages;
    activeAdminPage: AdminPages;
    linkTo: string;
}

function AdminPagesButton({text, activeAdminPage, linkTo}: AdminPagesButtonProps) {
    const active = text === AdminPages[activeAdminPage];
    return (
            <Link to={linkTo}
            className={`px-4 py-2 rounded-md cursor-pointer
                        ${active
                        ? 'bg-brand-600 text-white hover:bg-brand-800' 
                        : 'bg-neutral-50 hover:text-brand-500'}`}>
                {text}
            </Link>
    )
}
