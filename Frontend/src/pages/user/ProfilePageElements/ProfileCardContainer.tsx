export default function ProfileCardContainer({children}: {children: any}) {
    return (
      <div className="p-6 bg-white border rounded-lg shadow-md mx-auto w-fit">
        {children}
      </div>
    )
  }