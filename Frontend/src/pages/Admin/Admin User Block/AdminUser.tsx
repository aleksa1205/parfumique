import { AdminActionCardContainer } from "../../../components/Admin/AdminActionCardContainer";
import { adminUserCardsData } from "./AdminUserCardsData";

export default function AdminUser() {
    return (
        <AdminActionCardContainer data={adminUserCardsData} />
    )
}