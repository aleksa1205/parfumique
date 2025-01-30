import { AdminActionCardContainer } from "../../../components/Admin/AdminActionCardContainer";
import { adminPerfumerCardsData } from "./AdminPerfumerCardsData";

export default function AdminPerfumer() {
    return (
        <AdminActionCardContainer data={adminPerfumerCardsData} />
    )
}